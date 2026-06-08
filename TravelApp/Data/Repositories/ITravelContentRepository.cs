using System.Collections.Generic;
using System.Threading.Tasks;
using TravelApp.Models;
using TravelApp.Models.Enums;

namespace TravelApp.Data.Repositories
{
    public interface ITravelContentRepository
    {
        Task<IReadOnlyList<DestinationModel>> GetDestinationsAsync();
        Task<IReadOnlyList<DestinationModel>> GetApprovedDestinationsAsync();
        Task<IReadOnlyList<DestinationModel>> GetDestinationsByGuideAsync(
            int guideId);
        Task<IReadOnlyList<HotelModel>> GetHotelsAsync();
        Task<IReadOnlyList<HotelModel>> GetHotelsByGuideAsync(int guideId);
        Task<IReadOnlyList<BookingModel>> GetBookingsAsync();
        Task<bool> UpdateBookingStatusAsync(
            int bookingId,
            BookingStatus status);
        Task<bool> CreateDestinationAsync(DestinationModel destination);
        Task<bool> UpdateDestinationAsync(DestinationModel destination);
        Task<bool> UpdateDestinationByGuideAsync(
            DestinationModel destination,
            int guideId);
        Task<bool> DeleteDestinationAsync(int destinationId);
        Task<bool> UpdateDestinationApprovalAsync(
            int destinationId,
            ContentApprovalStatus status);
        Task<bool> CreateHotelAsync(HotelModel hotel);
        Task<bool> UpdateHotelAsync(HotelModel hotel);
        Task<bool> UpdateHotelByGuideAsync(HotelModel hotel, int guideId);
        Task<bool> DeleteHotelAsync(int hotelId);
        Task<bool> UpdateHotelApprovalAsync(
            int hotelId,
            ContentApprovalStatus status);
    }
}
