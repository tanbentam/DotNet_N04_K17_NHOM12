using System.Data.Entity.Migrations;
using MySql.Data.EntityFramework;

namespace TravelApp.Data.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "TravelApp.Data.ApplicationDbContext";
            MigrationsDirectory = @"Data\Migrations";

            SetSqlGenerator(
                "MySql.Data.MySqlClient",
                new MySqlMigrationSqlGenerator());
        }
    }
}
