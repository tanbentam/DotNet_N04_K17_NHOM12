namespace TravelApp.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPayments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Payments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BookingId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Method = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        TransactionCode = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                        ReferenceCode = c.String(maxLength: 100, storeType: "nvarchar"),
                        CreatedAt = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Bookings", t => t.BookingId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.BookingId)
                .Index(t => t.UserId)
                .Index(t => t.TransactionCode, unique: true, name: "UX_Payments_TransactionCode");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Payments", "UserId", "dbo.Users");
            DropForeignKey("dbo.Payments", "BookingId", "dbo.Bookings");
            DropIndex("dbo.Payments", "UX_Payments_TransactionCode");
            DropIndex("dbo.Payments", new[] { "UserId" });
            DropIndex("dbo.Payments", new[] { "BookingId" });
            DropTable("dbo.Payments");
        }
    }
}
