using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using TravelApp.Models;

namespace TravelApp.Data.Repositories
{
    public sealed class TravelContentRepository : ITravelContentRepository
    {
        public async Task<IReadOnlyList<DestinationModel>> GetDestinationsAsync()
        {
            using (var context = new ApplicationDbContext())
            {
                return await context.Destinations
                    .AsNoTracking()
                    .OrderBy(destination => destination.Name)
                    .ToListAsync();
            }
        }

        public async Task<IReadOnlyList<HotelModel>> GetHotelsAsync()
        {
            using (var context = new ApplicationDbContext())
            {
                return await context.Hotels
                    .AsNoTracking()
                    .Include(hotel => hotel.Destination)
                    .OrderBy(hotel => hotel.Name)
                    .ToListAsync();
            }
        }

        public async Task<IReadOnlyList<BookingModel>> GetBookingsAsync()
        {
            using (var context = new ApplicationDbContext())
            {
                return await context.Bookings
                    .AsNoTracking()
                    .Include(booking => booking.User)
                    .Include(booking => booking.Guide)
                    .Include(booking => booking.Hotel)
                    .Include(booking => booking.Destination)
                    .OrderByDescending(booking => booking.StartDate)
                    .ToListAsync();
            }
        }
    }
}
