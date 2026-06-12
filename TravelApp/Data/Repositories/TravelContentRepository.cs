using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using TravelApp.Models;
using TravelApp.Models.Enums;
using TravelApp.Services.Logging;

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

        public async Task<IReadOnlyList<DestinationModel>>
            GetApprovedDestinationsAsync()
        {
            using (var context = new ApplicationDbContext())
            {
                return await context.Destinations
                    .AsNoTracking()
                    .Where(destination =>
                        destination.ApprovalStatus ==
                            ContentApprovalStatus.Approved)
                    .OrderBy(destination => destination.Name)
                    .ToListAsync();
            }
        }

        public async Task<IReadOnlyList<DestinationModel>>
            SearchApprovedDestinationsAsync(
                string location,
                decimal minimumRating,
                string guideName)
        {
            using (var context = new ApplicationDbContext())
            {
                var query = context.Destinations
                    .AsNoTracking()
                    .Include(destination => destination.CreatedByGuide)
                    .Where(destination =>
                        destination.ApprovalStatus ==
                            ContentApprovalStatus.Approved);

                if (!string.IsNullOrWhiteSpace(location))
                {
                    var locationFilter = location.Trim();
                    query = query.Where(destination =>
                        destination.Name.Contains(locationFilter) ||
                        destination.Country.Contains(locationFilter));
                }

                if (minimumRating > 0)
                {
                    query = query.Where(destination =>
                        destination.AverageRating >= minimumRating);
                }

                if (!string.IsNullOrWhiteSpace(guideName))
                {
                    var guideFilter = guideName.Trim();
                    query = query.Where(destination =>
                        destination.CreatedByGuide != null &&
                        destination.CreatedByGuide.FullName.Contains(
                            guideFilter));
                }

                return await query
                    .OrderBy(destination => destination.Name)
                    .ToListAsync();
            }
        }

        public async Task<IReadOnlyList<HotelModel>> SearchApprovedHotelsAsync(
            string location,
            decimal maximumPrice,
            decimal minimumRating,
            string guideName)
        {
            using (var context = new ApplicationDbContext())
            {
                var query = context.Hotels
                    .AsNoTracking()
                    .Include(hotel => hotel.Destination)
                    .Include(hotel => hotel.CreatedByGuide)
                    .Where(hotel =>
                        hotel.ApprovalStatus ==
                            ContentApprovalStatus.Approved &&
                        hotel.Destination.ApprovalStatus ==
                            ContentApprovalStatus.Approved);

                if (!string.IsNullOrWhiteSpace(location))
                {
                    var locationFilter = location.Trim();
                    query = query.Where(hotel =>
                        hotel.Name.Contains(locationFilter) ||
                        hotel.Address.Contains(locationFilter) ||
                        hotel.Destination.Name.Contains(locationFilter) ||
                        hotel.Destination.Country.Contains(locationFilter));
                }

                if (maximumPrice > 0)
                {
                    query = query.Where(hotel =>
                        hotel.PricePerNight <= maximumPrice);
                }

                if (minimumRating > 0)
                {
                    query = query.Where(hotel =>
                        hotel.Rating >= minimumRating);
                }

                if (!string.IsNullOrWhiteSpace(guideName))
                {
                    var guideFilter = guideName.Trim();
                    query = query.Where(hotel =>
                        hotel.CreatedByGuide != null &&
                        hotel.CreatedByGuide.FullName.Contains(guideFilter));
                }

                return await query
                    .OrderBy(hotel => hotel.Name)
                    .ToListAsync();
            }
        }

        public async Task<IReadOnlyList<UserModel>> SearchGuidesAsync(
            string guideName,
            string availability)
        {
            using (var context = new ApplicationDbContext())
            {
                var query = context.Users
                    .AsNoTracking()
                    .Where(user => user.Role == RoleType.TourGuide);

                if (!string.IsNullOrWhiteSpace(guideName))
                {
                    var guideFilter = guideName.Trim();
                    query = query.Where(user =>
                        user.FullName.Contains(guideFilter));
                }

                if (!string.IsNullOrWhiteSpace(availability))
                {
                    var availabilityFilter = availability.Trim();
                    query = query.Where(user =>
                        user.Availabilities.Any(item =>
                            item.IsAvailable &&
                            (item.DayName.Contains(availabilityFilter) ||
                             item.TimeSlot.Contains(availabilityFilter))));
                }

                return await query
                    .OrderBy(user => user.FullName)
                    .ToListAsync();
            }
        }

        public async Task<IReadOnlyList<DestinationModel>>
            GetDestinationsByGuideAsync(int guideId)
        {
            if (guideId <= 0)
            {
                return new List<DestinationModel>();
            }

            using (var context = new ApplicationDbContext())
            {
                return await context.Destinations
                    .AsNoTracking()
                    .Where(destination =>
                        destination.CreatedByGuideId == guideId)
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

        public async Task<IReadOnlyList<BookingModel>> GetBookingsByUserAsync(
            int userId)
        {
            if (userId <= 0)
            {
                return new List<BookingModel>();
            }

            using (var context = new ApplicationDbContext())
            {
                return await context.Bookings
                    .AsNoTracking()
                    .Include(booking => booking.Guide)
                    .Include(booking => booking.Hotel)
                    .Include(booking => booking.Destination)
                    .Where(booking => booking.UserId == userId)
                    .OrderByDescending(booking => booking.StartDate)
                    .ThenByDescending(booking => booking.Id)
                    .ToListAsync();
            }
        }

        public async Task<bool> CreateBookingAsync(BookingModel booking)
        {
            if (booking == null ||
                booking.UserId <= 0 ||
                booking.GuideId <= 0 ||
                booking.DestinationId <= 0 ||
                booking.Nights <= 0 ||
                booking.StartDate.Date < System.DateTime.Today ||
                string.IsNullOrWhiteSpace(booking.BookingId))
            {
                return false;
            }

            using (var context = new ApplicationDbContext())
            {
                var userExists = await context.Users.AnyAsync(user =>
                    user.Id == booking.UserId &&
                    user.Role == RoleType.User);
                var guideExists = await context.Users.AnyAsync(guide =>
                    guide.Id == booking.GuideId &&
                    guide.Role == RoleType.TourGuide);
                var destinationExists =
                    await context.Destinations.AnyAsync(destination =>
                        destination.Id == booking.DestinationId &&
                        destination.ApprovalStatus ==
                            ContentApprovalStatus.Approved);

                if (!userExists || !guideExists || !destinationExists)
                {
                    return false;
                }

                if (booking.HotelId.HasValue)
                {
                    var hotelExists = await context.Hotels.AnyAsync(hotel =>
                        hotel.Id == booking.HotelId.Value &&
                        hotel.DestinationId == booking.DestinationId &&
                        hotel.ApprovalStatus ==
                            ContentApprovalStatus.Approved);
                    if (!hotelExists)
                    {
                        return false;
                    }
                }

                booking.Status = BookingStatus.Pending;
                context.Bookings.Add(booking);
                return await SaveChangesAsync(context);
            }
        }

        public async Task<bool> CancelBookingByUserAsync(
            int bookingId,
            int userId)
        {
            if (bookingId <= 0 || userId <= 0)
            {
                return false;
            }

            using (var context = new ApplicationDbContext())
            {
                var booking = await context.Bookings.FirstOrDefaultAsync(item =>
                    item.Id == bookingId &&
                    item.UserId == userId &&
                    (item.Status == BookingStatus.Pending ||
                     item.Status == BookingStatus.Accepted));
                if (booking == null)
                {
                    return false;
                }

                booking.Status = BookingStatus.Cancelled;
                return await SaveChangesAsync(context);
            }
        }

        public async Task<IReadOnlyList<BookingModel>>
            GetPendingBookingsByGuideAsync(int guideId)
        {
            if (guideId <= 0)
            {
                return new List<BookingModel>();
            }

            using (var context = new ApplicationDbContext())
            {
                var bookings = await context.Bookings
                    .AsNoTracking()
                    .Include(booking => booking.User)
                    .Include(booking => booking.Destination)
                    .Include(booking => booking.Hotel)
                    .Where(booking =>
                        booking.GuideId == guideId &&
                        booking.Status == BookingStatus.Pending)
                    .OrderBy(booking => booking.StartDate)
                    .ToListAsync();

                foreach (var booking in bookings)
                {
                    if (string.IsNullOrWhiteSpace(booking.UserName))
                    {
                        booking.UserName = booking.User?.FullName;
                    }

                    if (string.IsNullOrWhiteSpace(booking.DestinationName))
                    {
                        booking.DestinationName = booking.Destination?.Name;
                    }
                }

                return bookings;
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

        public async Task<IReadOnlyList<HotelModel>> GetHotelsByGuideAsync(
            int guideId)
        {
            if (guideId <= 0)
            {
                return new List<HotelModel>();
            }

            using (var context = new ApplicationDbContext())
            {
                return await context.Hotels
                    .AsNoTracking()
                    .Include(hotel => hotel.Destination)
                    .Where(hotel => hotel.CreatedByGuideId == guideId)
                    .OrderBy(hotel => hotel.Name)
                    .ToListAsync();
            }
        }

        public async Task<bool> UpdateDestinationByGuideAsync(
            DestinationModel destination,
            int guideId)
        {
            if (destination == null || destination.Id <= 0 || guideId <= 0)
            {
                return false;
            }

            using (var context = new ApplicationDbContext())
            {
                var existing = await context.Destinations
                    .FirstOrDefaultAsync(item =>
                        item.Id == destination.Id &&
                        item.CreatedByGuideId == guideId);
                if (existing == null)
                {
                    return false;
                }

                existing.Name = destination.Name;
                existing.Country = destination.Country;
                existing.Description = destination.Description;
                existing.ImageUrl = destination.ImageUrl;
                existing.ApprovalStatus = ContentApprovalStatus.Pending;
                return await SaveChangesAsync(context);
            }
        }

        public async Task<bool> UpdatePendingBookingByGuideAsync(
            int bookingId,
            int guideId,
            BookingStatus status)
        {
            if (bookingId <= 0 ||
                guideId <= 0 ||
                (status != BookingStatus.Accepted &&
                 status != BookingStatus.Rejected))
            {
                return false;
            }

            using (var context = new ApplicationDbContext())
            {
                var booking = await context.Bookings.FirstOrDefaultAsync(
                    item =>
                        item.Id == bookingId &&
                        item.GuideId == guideId &&
                        item.Status == BookingStatus.Pending);
                if (booking == null)
                {
                    return false;
                }

                booking.Status = status;
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

        public async Task<bool> UpdateHotelByGuideAsync(
            HotelModel hotel,
            int guideId)
        {
            if (hotel == null || hotel.Id <= 0 || guideId <= 0)
            {
                return false;
            }

            using (var context = new ApplicationDbContext())
            {
                var existing = await context.Hotels
                    .FirstOrDefaultAsync(item =>
                        item.Id == hotel.Id &&
                        item.CreatedByGuideId == guideId);
                var destinationExists = await context.Destinations.AnyAsync(
                    destination =>
                        destination.Id == hotel.DestinationId &&
                        destination.ApprovalStatus ==
                            ContentApprovalStatus.Approved);
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
                existing.ApprovalStatus = ContentApprovalStatus.Pending;
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
            catch (DbUpdateException ex)
            {
                LoggerService.LogException(
                    "Save travel content repository",
                    ex);
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
