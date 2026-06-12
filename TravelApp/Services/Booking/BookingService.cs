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
                    "Thông tin booking không hợp lệ.");
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
                        "User, Guide hoặc điểm đến không hợp lệ.");
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
                        "Guide không có lịch trống cho toàn bộ chuyến đi.");
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
                    "Đã tạo booking " + booking.BookingId + ".");
            }
        }

        public async Task<BookingOperationResult> CancelByUserAsync(
            int bookingId,
            int userId)
        {
            using (var context = new ApplicationDbContext())
            {
                var booking = await context.Bookings.FirstOrDefaultAsync(item =>
                    item.Id == bookingId && item.UserId == userId);
                if (booking == null)
                {
                    return BookingOperationResult.Failure(
                        "Không tìm thấy booking của User hiện tại.");
                }

                if (!CanChangeStatus(
                    booking.Status,
                    BookingStatus.Cancelled,
                    false))
                {
                    return BookingOperationResult.Failure(
                        "Chỉ có thể hủy booking đang chờ hoặc đã được chấp nhận.");
                }

                booking.Status = BookingStatus.Cancelled;
                await context.SaveChangesAsync();
                return BookingOperationResult.Success("Đã hủy booking.");
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
                    "Guide chỉ có thể chấp nhận hoặc từ chối booking.");
            }

            using (var context = new ApplicationDbContext())
            {
                var booking = await context.Bookings.FirstOrDefaultAsync(item =>
                    item.Id == bookingId && item.GuideId == guideId);
                if (booking == null || booking.Status != BookingStatus.Pending)
                {
                    return BookingOperationResult.Failure(
                        "Booking không còn chờ xử lý hoặc không thuộc Guide hiện tại.");
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
                        "Lịch Guide đã bị trùng hoặc không còn khả dụng.");
                }

                booking.Status = status;
                await context.SaveChangesAsync();
                return BookingOperationResult.Success(
                    status == BookingStatus.Accepted
                        ? "Đã chấp nhận booking."
                        : "Đã từ chối booking.");
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
                        "Không tìm thấy booking.");
                }

                if (!CanChangeStatus(booking.Status, status, true))
                {
                    return BookingOperationResult.Failure(
                        "Không thể chuyển booking từ trạng thái hiện tại.");
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
                        "Booking bị trùng lịch của User hoặc Guide.");
                }

                booking.Status = status;
                await context.SaveChangesAsync();
                return BookingOperationResult.Success(
                    "Đã cập nhật trạng thái booking.");
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
