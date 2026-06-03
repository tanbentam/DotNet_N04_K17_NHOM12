using System.Data.Entity;
using TravelApp.Common.Models;

namespace TravelApp.Backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("DefaultConnection")
        {
        }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<BookingModel> Bookings { get; set; }
    }
}