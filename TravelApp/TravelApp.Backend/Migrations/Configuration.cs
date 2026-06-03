using System.Data.Entity.Migrations;

namespace TravelApp.Backend.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<TravelApp.Backend.Data.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = "Migrations";
        }

        protected override void Seed(TravelApp.Backend.Data.ApplicationDbContext context)
        {
            // Seed initial data if required. This method is called after migrating to the latest version.
        }
    }
}
