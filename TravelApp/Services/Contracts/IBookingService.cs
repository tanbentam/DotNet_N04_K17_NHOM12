using System.Threading.Tasks;
using TravelApp.Models;
using TravelApp.Models.Enums;
using TravelApp.Services.Booking;

namespace TravelApp.Services.Contracts
{
    public interface IBookingService
    {
        BookingPriceQuote CalculatePrice(decimal hotelPricePerNight, int days);
        Task<BookingOperationResult> CreateBookingAsync(BookingModel booking);
        Task<BookingOperationResult> CancelByUserAsync(
            int bookingId,
            int userId);
        Task<BookingOperationResult> UpdateByGuideAsync(
            int bookingId,
            int guideId,
            BookingStatus status);
        Task<BookingOperationResult> UpdateByAdminAsync(
            int bookingId,
            BookingStatus status);
    }
}
