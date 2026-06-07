using System.Collections.Generic;
using System.Threading.Tasks;
using TravelApp.Models;

namespace TravelApp.Data.Repositories
{
    public interface ITravelContentRepository
    {
        Task<IReadOnlyList<DestinationModel>> GetDestinationsAsync();
        Task<IReadOnlyList<HotelModel>> GetHotelsAsync();
        Task<IReadOnlyList<BookingModel>> GetBookingsAsync();
    }
}
