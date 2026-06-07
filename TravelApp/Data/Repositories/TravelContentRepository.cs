using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using TravelApp.Models;
using TravelApp.Models.Enums;

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
                    .Include(destination => destination.CreatedByGuide)
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
                    .Include(hotel => hotel.CreatedByGuide)
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

        public async Task<bool> UpdateBookingStatusAsync(
            int bookingId,
            BookingStatus status)
        {
            using (var context = new ApplicationDbContext())
            {
                var booking = await context.Bookings.FindAsync(bookingId);
                if (booking == null)
                {
                    return false;
                }

                booking.Status = status;
                return await SaveChangesAsync(context);
            }
        }

        public async Task<bool> CreateDestinationAsync(
            DestinationModel destination)
        {
            if (destination == null)
            {
                return false;
            }

            using (var context = new ApplicationDbContext())
            {
                context.Destinations.Add(destination);
                return await SaveChangesAsync(context);
            }
        }

        public async Task<bool> UpdateDestinationAsync(
            DestinationModel destination)
        {
            if (destination == null || destination.Id <= 0)
            {
                return false;
            }

            using (var context = new ApplicationDbContext())
            {
                var existing = await context.Destinations.FindAsync(destination.Id);
                if (existing == null)
                {
                    return false;
                }

                existing.Name = destination.Name;
                existing.Country = destination.Country;
                existing.Description = destination.Description;
                existing.ImageUrl = destination.ImageUrl;
                existing.AverageRating = destination.AverageRating;
                return await SaveChangesAsync(context);
            }
        }

        public async Task<bool> DeleteDestinationAsync(int destinationId)
        {
            using (var context = new ApplicationDbContext())
            {
                var destination = await context.Destinations.FindAsync(destinationId);
                if (destination == null)
                {
                    return false;
                }

                var hasHotels = await context.Hotels.AnyAsync(
                    hotel => hotel.DestinationId == destinationId);
                var hasBookings = await context.Bookings.AnyAsync(
                    booking => booking.DestinationId == destinationId);
                if (hasHotels || hasBookings)
                {
                    return false;
                }

                context.Destinations.Remove(destination);
                return await SaveChangesAsync(context);
            }
        }

        public async Task<bool> CreateHotelAsync(HotelModel hotel)
        {
            if (hotel == null)
            {
                return false;
            }

            using (var context = new ApplicationDbContext())
            {
                var destinationExists = await context.Destinations.AnyAsync(
                    destination => destination.Id == hotel.DestinationId);
                if (!destinationExists)
                {
                    return false;
                }

                context.Hotels.Add(hotel);
                return await SaveChangesAsync(context);
            }
        }

        public async Task<bool> UpdateDestinationApprovalAsync(
            int destinationId,
            ContentApprovalStatus status)
        {
            using (var context = new ApplicationDbContext())
            {
                var destination = await context.Destinations.FindAsync(destinationId);
                if (destination == null)
                {
                    return false;
                }

                destination.ApprovalStatus = status;
                return await SaveChangesAsync(context);
            }
        }

        public async Task<bool> UpdateHotelAsync(HotelModel hotel)
        {
            if (hotel == null || hotel.Id <= 0)
            {
                return false;
            }

            using (var context = new ApplicationDbContext())
            {
                var existing = await context.Hotels.FindAsync(hotel.Id);
                var destinationExists = await context.Destinations.AnyAsync(
                    destination => destination.Id == hotel.DestinationId);
                if (existing == null || !destinationExists)
                {
                    return false;
                }

                existing.DestinationId = hotel.DestinationId;
                existing.Name = hotel.Name;
                existing.Address = hotel.Address;
                existing.Description = hotel.Description;
                existing.PricePerNight = hotel.PricePerNight;
                existing.Rating = hotel.Rating;
                existing.ImageUrl = hotel.ImageUrl;
                return await SaveChangesAsync(context);
            }
        }

        public async Task<bool> DeleteHotelAsync(int hotelId)
        {
            using (var context = new ApplicationDbContext())
            {
                var hotel = await context.Hotels.FindAsync(hotelId);
                if (hotel == null)
                {
                    return false;
                }

                var hasBookings = await context.Bookings.AnyAsync(
                    booking => booking.HotelId == hotelId);
                if (hasBookings)
                {
                    return false;
                }

                context.Hotels.Remove(hotel);
                return await SaveChangesAsync(context);
            }
        }

        private static async Task<bool> SaveChangesAsync(
            ApplicationDbContext context)
        {
            try
            {
                await context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public async Task<bool> UpdateHotelApprovalAsync(
            int hotelId,
            ContentApprovalStatus status)
        {
            using (var context = new ApplicationDbContext())
            {
                var hotel = await context.Hotels.FindAsync(hotelId);
                if (hotel == null)
                {
                    return false;
                }

                hotel.ApprovalStatus = status;
                return await SaveChangesAsync(context);
            }
        }
    }
}
