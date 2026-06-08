namespace TravelApp.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SyncMySqlModelSnapshot : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Bookings", "StartDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.GuideAvailabilities", "UpdatedAt", c => c.DateTime(nullable: false, precision: 0));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.GuideAvailabilities", "UpdatedAt", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Bookings", "StartDate", c => c.DateTime(nullable: false));
        }
    }
}
