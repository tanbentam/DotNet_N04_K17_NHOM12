using System.Data.Entity;
using TravelApp.Common.Models;

namespace TravelApp.Backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        static ApplicationDbContext()
        {
            Database.SetInitializer(new System.Data.Entity.Migrations.MigrateDatabaseToLatestVersion<ApplicationDbContext, Migrations.Configuration>());
        }

        public ApplicationDbContext() : base("name=DefaultConnection")
        {
        }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<BookingModel> Bookings { get; set; }
    }
}
