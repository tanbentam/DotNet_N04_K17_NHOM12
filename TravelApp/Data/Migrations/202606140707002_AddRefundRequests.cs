namespace TravelApp.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRefundRequests : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bookings", "RefundRequestedAt", c => c.DateTime(precision: 0));
            AddColumn("dbo.Bookings", "RefundReason", c => c.String(maxLength: 500, storeType: "nvarchar"));
            AddColumn("dbo.Bookings", "RefundResolvedAt", c => c.DateTime(precision: 0));
            AddColumn("dbo.Bookings", "RefundApproved", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Bookings", "RefundApproved");
            DropColumn("dbo.Bookings", "RefundResolvedAt");
            DropColumn("dbo.Bookings", "RefundReason");
            DropColumn("dbo.Bookings", "RefundRequestedAt");
        }
    }
}
