using System.Data.Entity.Migrations;
using TravelApp.Common.Models;

namespace TravelApp.Backend.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<TravelApp.Backend.Data.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = false;
            ContextKey = "TravelApp.Backend.Data.ApplicationDbContext";
        }

        protected override void Seed(TravelApp.Backend.Data.ApplicationDbContext context)
        {
            context.Users.AddOrUpdate(
                u => u.Email,
                new UserModel
                {
                    Id = 1,
                    Email = "admin@travelapp.local",
                    Phone = "0000000000",
                    PasswordHash = "CHANGE_ME",
                    FullName = "Administrator"
                });
        }
    }
}
