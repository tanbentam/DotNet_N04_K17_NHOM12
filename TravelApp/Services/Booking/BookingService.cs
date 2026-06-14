using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using TravelApp.Data;
using TravelApp.Models;
using TravelApp.Models.Enums;
using TravelApp.Services.Contracts;

namespace TravelApp.Services.Booking
{
    public sealed class BookingService : IBookingService
    {
        public const decimal GuideFeePerDay = 500000m;
        public const decimal ServiceFeeRate = 0.05m;
        public const decimal LongTripDiscountRate = 0.10m;
        public const int LongTripMinimumDays = 7;
        public const int MaximumTripDays = 30;
        public const int MaximumAdvanceBookingDays = 365;

        public BookingPriceQuote CalculatePrice(
            decimal hotelPricePerNight,
            int days)
        {
            if (days <= 0 || days > MaximumTripDays)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(days),
                    "Số ngày phải từ 1 đến 30.");
            }

            if (hotelPricePerNight < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(hotelPricePerNight),
                    "Giá phòng không được âm.");
            }

            var guideFee = GuideFeePerDay * days;
            var hotelFee = hotelPricePerNight * days;
            var subtotal = guideFee + hotelFee;
            var discount = days >= LongTripMinimumDays
                ? Math.Round(
                    subtotal * LongTripDiscountRate,
                    2,
                    MidpointRounding.AwayFromZero)
                : 0;
            var discountedSubtotal = subtotal - discount;
            var serviceFee = Math.Round(
                discountedSubtotal * ServiceFeeRate,
                2,
                MidpointRounding.AwayFromZero);

            return new BookingPriceQuote
            {
                GuideFee = guideFee,
                HotelFee = hotelFee,
                Discount = discount,
                ServiceFee = serviceFee,
                Total = discountedSubtotal + serviceFee
            };
        }

        public async Task<int> CompleteExpiredBookingsAsync()
        {
            var today = DateTime.Today;
            using (var context = new ApplicationDbContext())
            {
                var candidates = await context.Bookings
                    .Where(booking =>
                        booking.Status == BookingStatus.Paid &&
                        booking.StartDate < today &&
                        (!booking.RefundRequestedAt.HasValue ||
                         booking.RefundResolvedAt.HasValue))
                    .ToListAsync();
                var expiredBookings = candidates
                    .Where(booking => ShouldAutoComplete(booking, today))
                    .ToList();

                foreach (var booking in expiredBookings)
                {
                    booking.Status = BookingStatus.Completed;
                }

                if (expiredBookings.Count > 0)
                {
                    await context.SaveChangesAsync();
                }

                return expiredBookings.Count;
            }
        }

        public static bool ShouldAutoComplete(
            BookingModel booking,
            DateTime today)
        {
            return booking != null &&
                booking.Status == BookingStatus.Paid &&
                !booking.HasPendingRefundRequest &&
                today.Date > booking.CompletionDate;
        }

        public async Task<BookingOperationResult> CreateBookingAsync(
            BookingModel booking)
        {
            if (booking == null ||
                booking.UserId <= 0 ||
                booking.GuideId <= 0 ||
                booking.DestinationId <= 0 ||
                string.IsNullOrWhiteSpace(booking.BookingId))
            {
                return BookingOperationResult.Failure(
                    "Thông tin đặt tour không hợp lệ.");
            }

            var validation = ValidateDates(booking.StartDate, booking.Nights);
            if (validation != null)
            {
                return BookingOperationResult.Failure(validation);
            }

            using (var context = new ApplicationDbContext())
            {
                var user = await context.Users.AsNoTracking()
                    .FirstOrDefaultAsync(item =>
                        item.Id == booking.UserId &&
                        item.Role == RoleType.User);
                var guide = await context.Users.AsNoTracking()
                    .FirstOrDefaultAsync(item =>
                        item.Id == booking.GuideId &&
                        item.Role == RoleType.TourGuide);
                var destination = await context.Destinations.AsNoTracking()
                    .FirstOrDefaultAsync(item =>
                        item.Id == booking.DestinationId &&
                        item.ApprovalStatus == ContentApprovalStatus.Approved);

                if (user == null || guide == null || destination == null)
                {
                    return BookingOperationResult.Failure(
                        "Người dùng, hướng dẫn viên hoặc điểm đến không hợp lệ.");
                }

                decimal hotelPrice = 0;
                if (booking.HotelId.HasValue)
                {
                    var hotel = await context.Hotels.AsNoTracking()
                        .FirstOrDefaultAsync(item =>
                            item.Id == booking.HotelId.Value &&
                            item.DestinationId == booking.DestinationId &&
                            item.ApprovalStatus ==
                                ContentApprovalStatus.Approved);
                    if (hotel == null)
                    {
                        return BookingOperationResult.Failure(
                            "Khách sạn không thuộc điểm đến hoặc chưa được duyệt.");
                    }

                    hotelPrice = hotel.PricePerNight;
                }

                if (!await IsGuideAvailableAsync(
                    context,
                    booking.GuideId,
                    booking.StartDate.Date,
                    booking.Nights,
                    null))
                {
                    return BookingOperationResult.Failure(
                        "Hướng dẫn viên không có lịch trống cho toàn bộ chuyến đi.");
                }

                if (await HasUserConflictAsync(
                    context,
                    booking.UserId,
                    booking.StartDate.Date,
                    booking.Nights,
                    null))
                {
                    return BookingOperationResult.Failure(
                        "Bạn đã có một chuyến đi khác trùng thời gian.");
                }

                var quote = CalculatePrice(hotelPrice, booking.Nights);
                booking.StartDate = booking.StartDate.Date;
                booking.Price = quote.Total;
                booking.Status = BookingStatus.Pending;
                booking.DestinationName = destination.Name;
                booking.UserName = user.FullName;

                context.Bookings.Add(booking);
                await context.SaveChangesAsync();
                return BookingOperationResult.Success(
                    "Đã tạo đặt tour " + booking.BookingId + ".");
            }
        }

        public async Task<BookingOperationResult> CancelByUserAsync(
            int bookingId,
            int userId,
            string reason)
        {
            using (var context = new ApplicationDbContext())
            {
                var booking = await context.Bookings.FirstOrDefaultAsync(item =>
                    item.Id == bookingId && item.UserId == userId);
                if (booking == null)
                {
                    return BookingOperationResult.Failure(
                        "Không tìm thấy đặt tour của người dùng hiện tại.");
                }

                if (booking.Status == BookingStatus.Paid)
                {
                    var normalizedReason = reason?.Trim();
                    if (string.IsNullOrWhiteSpace(normalizedReason) ||
                        normalizedReason.Length < 10 ||
                        normalizedReason.Length > 500)
                    {
                        return BookingOperationResult.Failure(
                            "Lý do hoàn tiền phải từ 10 đến 500 ký tự.");
                    }

                    if (booking.StartDate.Date <= DateTime.Today)
                    {
                        return BookingOperationResult.Failure(
                            "Không thể yêu cầu hoàn tiền khi tour đã hoặc đang bắt đầu.");
                    }

                    if (booking.HasPendingRefundRequest)
                    {
                        return BookingOperationResult.Failure(
                            "Đặt tour đã có yêu cầu hoàn tiền đang chờ quản trị viên xử lý.");
                    }

                    booking.RefundRequestedAt = DateTime.Now;
                    booking.RefundReason = normalizedReason;
                    booking.RefundResolvedAt = null;
                    booking.RefundApproved = null;
                    await context.SaveChangesAsync();
                    return BookingOperationResult.Success(
                        "Đã gửi yêu cầu hủy và hoàn tiền đến quản trị viên.");
                }

                if (!CanChangeStatus(
                    booking.Status,
                    BookingStatus.Cancelled,
                    false))
                {
                    return BookingOperationResult.Failure(
                        "Chỉ có thể hủy đặt tour đang chờ hoặc đã được chấp nhận.");
                }

                booking.Status = BookingStatus.Cancelled;
                if (booking.GuideCancellationRequestedAt.HasValue &&
                    !booking.GuideCancellationResolvedAt.HasValue)
                {
                    booking.GuideCancellationResolvedAt = DateTime.Now;
                    booking.GuideCancellationApproved = true;
                }

                await context.SaveChangesAsync();
                return BookingOperationResult.Success("Đã hủy đặt tour.");
            }
        }

        public async Task<BookingOperationResult> ResolveRefundRequestAsync(
            int bookingId,
            bool approve)
        {
            using (var context = new ApplicationDbContext())
            using (var transaction = context.Database.BeginTransaction())
            {
                var booking = await context.Bookings.FindAsync(bookingId);
                if (booking == null || !booking.HasPendingRefundRequest)
                {
                    return BookingOperationResult.Failure(
                        "Đặt tour không có yêu cầu hoàn tiền đang chờ xử lý.");
                }

                if (booking.Status != BookingStatus.Paid)
                {
                    return BookingOperationResult.Failure(
                        "Đặt tour không còn ở trạng thái có thể hoàn tiền.");
                }

                booking.RefundResolvedAt = DateTime.Now;
                booking.RefundApproved = approve;
                if (approve)
                {
                    var payment = await context.Payments
                        .Where(item =>
                            item.BookingId == booking.Id &&
                            item.Status == PaymentStatus.Successful)
                        .OrderByDescending(item => item.CreatedAt)
                        .FirstOrDefaultAsync();
                    if (payment == null)
                    {
                        return BookingOperationResult.Failure(
                            "Không tìm thấy giao dịch thành công để hoàn tiền.");
                    }

                    payment.Status = PaymentStatus.Refunded;
                    booking.Status = BookingStatus.Cancelled;
                }

                await context.SaveChangesAsync();
                transaction.Commit();
                return BookingOperationResult.Success(
                    approve
                        ? "Đã duyệt hủy đặt tour và hoàn tiền mô phỏng."
                        : "Đã từ chối yêu cầu hoàn tiền.");
            }
        }

        public async Task<BookingOperationResult> UpdateByGuideAsync(
            int bookingId,
            int guideId,
            BookingStatus status)
        {
            if (status != BookingStatus.Accepted &&
                status != BookingStatus.Rejected)
            {
                return BookingOperationResult.Failure(
                    "Hướng dẫn viên chỉ có thể chấp nhận hoặc từ chối đặt tour.");
            }

            using (var context = new ApplicationDbContext())
            {
                var booking = await context.Bookings.FirstOrDefaultAsync(item =>
                    item.Id == bookingId && item.GuideId == guideId);
                if (booking == null || booking.Status != BookingStatus.Pending)
                {
                    return BookingOperationResult.Failure(
                        "Đặt tour không còn chờ xử lý hoặc không thuộc hướng dẫn viên hiện tại.");
                }

                if (status == BookingStatus.Accepted &&
                    !await IsGuideAvailableAsync(
                        context,
                        guideId,
                        booking.StartDate,
                        booking.Nights,
                        booking.Id))
                {
                    return BookingOperationResult.Failure(
                        "Lịch hướng dẫn viên đã bị trùng hoặc không còn khả dụng.");
                }

                booking.Status = status;
                await context.SaveChangesAsync();
                return BookingOperationResult.Success(
                    status == BookingStatus.Accepted
                        ? "Đã chấp nhận đặt tour."
                        : "Đã từ chối đặt tour.");
            }
        }

        public async Task<BookingOperationResult> UpdateByAdminAsync(
            int bookingId,
            BookingStatus status)
        {
            using (var context = new ApplicationDbContext())
            {
                var booking = await context.Bookings.FindAsync(bookingId);
                if (booking == null)
                {
                    return BookingOperationResult.Failure(
                        "Không tìm thấy đặt tour.");
                }

                if (!CanChangeStatus(booking.Status, status, true))
                {
                    return BookingOperationResult.Failure(
                        "Không thể chuyển đặt tour từ trạng thái hiện tại.");
                }

                if (booking.GuideCancellationRequestedAt.HasValue &&
                    !booking.GuideCancellationResolvedAt.HasValue)
                {
                    if (status != BookingStatus.Cancelled)
                    {
                        return BookingOperationResult.Failure(
                            "Hãy duyệt hoặc từ chối yêu cầu hủy của hướng dẫn viên trước.");
                    }

                    booking.GuideCancellationResolvedAt = DateTime.Now;
                    booking.GuideCancellationApproved = true;
                }

                if (booking.HasPendingRefundRequest)
                {
                    return BookingOperationResult.Failure(
                        "Hãy duyệt hoặc từ chối yêu cầu hoàn tiền trước.");
                }

                if (status == BookingStatus.Accepted &&
                    (!await IsGuideAvailableAsync(
                        context,
                        booking.GuideId,
                        booking.StartDate,
                        booking.Nights,
                        booking.Id) ||
                     await HasUserConflictAsync(
                        context,
                        booking.UserId,
                        booking.StartDate,
                        booking.Nights,
                        booking.Id)))
                {
                    return BookingOperationResult.Failure(
                        "Đặt tour bị trùng lịch của người dùng hoặc hướng dẫn viên.");
                }

                booking.Status = status;
                await context.SaveChangesAsync();
                return BookingOperationResult.Success(
                    "Đã cập nhật trạng thái đặt tour.");
            }
        }

        public async Task<BookingOperationResult>
            RequestCancellationByGuideAsync(
                int bookingId,
                int guideId,
                string reason)
        {
            var normalizedReason = reason?.Trim();
            if (string.IsNullOrWhiteSpace(normalizedReason) ||
                normalizedReason.Length < 10 ||
                normalizedReason.Length > 500)
            {
                return BookingOperationResult.Failure(
                    "Lý do hủy phải từ 10 đến 500 ký tự.");
            }

            using (var context = new ApplicationDbContext())
            {
                var booking = await context.Bookings.FirstOrDefaultAsync(item =>
                    item.Id == bookingId &&
                    item.GuideId == guideId);
                if (booking == null)
                {
                    return BookingOperationResult.Failure(
                        "Đặt tour không thuộc hướng dẫn viên hiện tại.");
                }

                if (booking.Status != BookingStatus.Accepted)
                {
                    return BookingOperationResult.Failure(
                        "Chỉ có thể gửi yêu cầu hủy đặt tour đã chấp nhận và chưa thanh toán.");
                }

                if (booking.StartDate.Date <= DateTime.Today)
                {
                    return BookingOperationResult.Failure(
                        "Không thể gửi yêu cầu hủy khi tour đã hoặc đang bắt đầu.");
                }

                if (booking.GuideCancellationRequestedAt.HasValue &&
                    !booking.GuideCancellationResolvedAt.HasValue)
                {
                    return BookingOperationResult.Failure(
                        "Đặt tour này đã có yêu cầu hủy đang chờ quản trị viên xử lý.");
                }

                booking.GuideCancellationRequestedAt = DateTime.Now;
                booking.GuideCancellationReason = normalizedReason;
                booking.GuideCancellationResolvedAt = null;
                booking.GuideCancellationApproved = null;
                await context.SaveChangesAsync();

                return BookingOperationResult.Success(
                    "Đã gửi yêu cầu hủy đặt tour đến quản trị viên.");
            }
        }

        public async Task<BookingOperationResult>
            ResolveGuideCancellationRequestAsync(
                int bookingId,
                bool approve)
        {
            using (var context = new ApplicationDbContext())
            {
                var booking = await context.Bookings.FindAsync(bookingId);
                if (booking == null ||
                    !booking.GuideCancellationRequestedAt.HasValue ||
                    booking.GuideCancellationResolvedAt.HasValue)
                {
                    return BookingOperationResult.Failure(
                        "Đặt tour không có yêu cầu hủy đang chờ xử lý.");
                }

                if (approve && booking.Status != BookingStatus.Accepted)
                {
                    return BookingOperationResult.Failure(
                        "Đặt tour không còn ở trạng thái có thể hủy.");
                }

                booking.GuideCancellationResolvedAt = DateTime.Now;
                booking.GuideCancellationApproved = approve;
                if (approve)
                {
                    booking.Status = BookingStatus.Cancelled;
                }

                await context.SaveChangesAsync();
                return BookingOperationResult.Success(
                    approve
                        ? "Đã duyệt yêu cầu hủy đặt tour."
                        : "Đã từ chối yêu cầu hủy đặt tour.");
            }
        }

        public static bool CanChangeStatus(
            BookingStatus current,
            BookingStatus next,
            bool isAdmin)
        {
            if (current == next)
            {
                return true;
            }

            switch (current)
            {
                case BookingStatus.Pending:
                    return next == BookingStatus.Accepted ||
                        next == BookingStatus.Rejected ||
                        next == BookingStatus.Cancelled;
                case BookingStatus.Accepted:
                    return next == BookingStatus.Paid ||
                        next == BookingStatus.Cancelled;
                case BookingStatus.Paid:
                    return isAdmin && next == BookingStatus.Completed;
                default:
                    return false;
            }
        }

        private static string ValidateDates(DateTime startDate, int days)
        {
            if (days <= 0 || days > MaximumTripDays)
            {
                return "Số ngày chuyến đi phải từ 1 đến 30.";
            }

            if (startDate.Date < DateTime.Today)
            {
                return "Ngày bắt đầu không được ở quá khứ.";
            }

            if (startDate.Date > DateTime.Today.AddDays(
                MaximumAdvanceBookingDays))
            {
                return "Chỉ có thể đặt tour trước tối đa 365 ngày.";
            }

            return null;
        }

        private static async Task<bool> IsGuideAvailableAsync(
            ApplicationDbContext context,
            int guideId,
            DateTime startDate,
            int days,
            int? excludedBookingId)
        {
            var endDate = startDate.Date.AddDays(days);
            var possibleConflicts = await context.Bookings
                .AsNoTracking()
                .Where(item =>
                    item.GuideId == guideId &&
                    (!excludedBookingId.HasValue ||
                     item.Id != excludedBookingId.Value) &&
                    (item.Status == BookingStatus.Accepted ||
                     item.Status == BookingStatus.Paid) &&
                    item.StartDate < endDate)
                .Select(item => new
                {
                    item.StartDate,
                    item.Nights
                })
                .ToListAsync();
            var hasConflict = possibleConflicts.Any(item =>
                item.StartDate.AddDays(item.Nights) > startDate.Date);
            if (hasConflict)
            {
                return false;
            }

            var availability = await context.GuideAvailabilities
                .AsNoTracking()
                .Where(item => item.GuideId == guideId)
                .ToListAsync();
            if (availability.Count == 0)
            {
                return true;
            }

            for (var offset = 0; offset < days; offset++)
            {
                var date = startDate.Date.AddDays(offset);
                var dayNumber = date.DayOfWeek == DayOfWeek.Sunday
                    ? 7
                    : (int)date.DayOfWeek;
                if (!availability.Any(item =>
                    item.DayOfWeek == dayNumber &&
                    item.IsAvailable))
                {
                    return false;
                }
            }

            return true;
        }

        private static Task<bool> HasUserConflictAsync(
            ApplicationDbContext context,
            int userId,
            DateTime startDate,
            int days,
            int? excludedBookingId)
        {
            var endDate = startDate.Date.AddDays(days);
            return HasUserConflictInMemoryAsync(
                context,
                userId,
                startDate,
                endDate,
                excludedBookingId);
        }

        private static async Task<bool> HasUserConflictInMemoryAsync(
            ApplicationDbContext context,
            int userId,
            DateTime startDate,
            DateTime endDate,
            int? excludedBookingId)
        {
            var possibleConflicts = await context.Bookings
                .AsNoTracking()
                .Where(item =>
                    item.UserId == userId &&
                    (!excludedBookingId.HasValue ||
                     item.Id != excludedBookingId.Value) &&
                    (item.Status == BookingStatus.Accepted ||
                     item.Status == BookingStatus.Paid) &&
                    item.StartDate < endDate)
                .Select(item => new
                {
                    item.StartDate,
                    item.Nights
                })
                .ToListAsync();

            return possibleConflicts.Any(item =>
                item.StartDate.AddDays(item.Nights) > startDate.Date);
        }
    }
}
