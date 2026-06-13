using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using TravelApp.Models;
using TravelApp.Models.Enums;
using TravelApp.Services.Logging;

namespace TravelApp.Data.Repositories
{
    public sealed class PaymentRepository : IPaymentRepository
    {
        public async Task<IReadOnlyList<BookingModel>>
            GetPayableBookingsAsync(int userId)
        {
            if (userId <= 0)
            {
                return new List<BookingModel>();
            }

            using (var context = new ApplicationDbContext())
            {
                return await context.Bookings
                    .AsNoTracking()
                    .Include(booking => booking.Destination)
                    .Include(booking => booking.Guide)
                    .Include(booking => booking.Hotel)
                    .Where(booking =>
                        booking.UserId == userId &&
                        booking.Status == BookingStatus.Accepted &&
                        (!booking.GuideCancellationRequestedAt.HasValue ||
                         booking.GuideCancellationResolvedAt.HasValue))
                    .OrderBy(booking => booking.StartDate)
                    .ToListAsync();
            }
        }

        public async Task<IReadOnlyList<PaymentModel>>
            GetPaymentHistoryAsync(int userId)
        {
            if (userId <= 0)
            {
                return new List<PaymentModel>();
            }

            using (var context = new ApplicationDbContext())
            {
                return await context.Payments
                    .AsNoTracking()
                    .Include(payment => payment.Booking)
                    .Include(payment => payment.Booking.Destination)
                    .Where(payment => payment.UserId == userId)
                    .OrderByDescending(payment => payment.CreatedAt)
                    .ThenByDescending(payment => payment.Id)
                    .ToListAsync();
            }
        }

        public async Task<PaymentProcessResult> ProcessPaymentAsync(
            int userId,
            int bookingId,
            PaymentMethod method,
            string transactionCode,
            string referenceCode,
            bool simulateSuccess)
        {
            if (userId <= 0 ||
                bookingId <= 0 ||
                string.IsNullOrWhiteSpace(transactionCode))
            {
                return PaymentProcessResult.Rejected(
                    "Thông tin giao dịch không hợp lệ.");
            }

            using (var context = new ApplicationDbContext())
            using (var transaction = context.Database.BeginTransaction(
                IsolationLevel.Serializable))
            {
                try
                {
                    var booking = await context.Bookings.FirstOrDefaultAsync(
                        item =>
                            item.Id == bookingId &&
                            item.UserId == userId);
                    if (booking == null)
                    {
                        return PaymentProcessResult.Rejected(
                            "Booking không thuộc User hiện tại.");
                    }

                    if (booking.Status != BookingStatus.Accepted)
                    {
                        return PaymentProcessResult.Rejected(
                            "Chỉ booking đã được Guide chấp nhận mới có thể thanh toán.");
                    }

                    if (booking.GuideCancellationRequestedAt.HasValue &&
                        !booking.GuideCancellationResolvedAt.HasValue)
                    {
                        return PaymentProcessResult.Rejected(
                            "Booking đang có yêu cầu hủy chờ Admin xử lý.");
                    }

                    var hasSuccessfulPayment =
                        await context.Payments.AnyAsync(payment =>
                            payment.BookingId == bookingId &&
                            payment.Status == PaymentStatus.Successful);
                    if (hasSuccessfulPayment)
                    {
                        return PaymentProcessResult.Rejected(
                            "Booking này đã được thanh toán.");
                    }

                    context.Payments.Add(new PaymentModel
                    {
                        BookingId = booking.Id,
                        UserId = userId,
                        Amount = booking.Price,
                        Method = method,
                        Status = simulateSuccess
                            ? PaymentStatus.Successful
                            : PaymentStatus.Failed,
                        TransactionCode = transactionCode.Trim(),
                        ReferenceCode = string.IsNullOrWhiteSpace(referenceCode)
                            ? null
                            : referenceCode.Trim(),
                        CreatedAt = DateTime.Now
                    });

                    if (simulateSuccess)
                    {
                        booking.Status = BookingStatus.Paid;
                    }

                    await context.SaveChangesAsync();
                    transaction.Commit();

                    return simulateSuccess
                        ? PaymentProcessResult.Success()
                        : PaymentProcessResult.FailedSimulation();
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    LoggerService.LogException(
                        "Save payment repository",
                        ex,
                        "UserId=" + userId +
                        "; BookingId=" + bookingId +
                        "; Method=" + method);
                    return PaymentProcessResult.Rejected(
                        "Không thể lưu giao dịch hoặc mã giao dịch đã tồn tại.");
                }
            }
        }
    }
}
