using System.Data.Entity;
using TravelApp.Frontend.Models;

namespace TravelApp.Frontend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("DefaultConnection")
        {
        }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<BookingModel> Bookings { get; set; }
        public DbSet<HotelModel> Hotels { get; set; }
        public DbSet<DestinationModel> Destinations { get; set; }
    }
}
