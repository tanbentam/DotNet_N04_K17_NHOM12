namespace TravelApp.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGuideCancellationRequests : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bookings", "GuideCancellationRequestedAt", c => c.DateTime(precision: 0));
            AddColumn("dbo.Bookings", "GuideCancellationReason", c => c.String(maxLength: 500, storeType: "nvarchar"));
            AddColumn("dbo.Bookings", "GuideCancellationResolvedAt", c => c.DateTime(precision: 0));
            AddColumn("dbo.Bookings", "GuideCancellationApproved", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Bookings", "GuideCancellationApproved");
            DropColumn("dbo.Bookings", "GuideCancellationResolvedAt");
            DropColumn("dbo.Bookings", "GuideCancellationReason");
            DropColumn("dbo.Bookings", "GuideCancellationRequestedAt");
        }
    }
}
