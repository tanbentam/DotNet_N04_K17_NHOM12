namespace TravelApp.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Bookings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        GuideId = c.Int(nullable: false),
                        HotelId = c.Int(),
                        DestinationId = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        Nights = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Status = c.Int(nullable: false),
                        BookingId = c.String(nullable: false, maxLength: 30),
                        DestinationName = c.String(maxLength: 150),
                        UserName = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Destinations", t => t.DestinationId)
                .ForeignKey("dbo.Users", t => t.GuideId)
                .ForeignKey("dbo.Hotels", t => t.HotelId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.GuideId)
                .Index(t => t.HotelId)
                .Index(t => t.DestinationId)
                .Index(t => t.BookingId, unique: true, name: "UX_Bookings_BookingId");
            
            CreateTable(
                "dbo.Destinations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 150),
                        Country = c.String(nullable: false, maxLength: 100),
                        Description = c.String(maxLength: 2000),
                        ImageUrl = c.String(maxLength: 500),
                        AverageRating = c.Decimal(nullable: false, precision: 3, scale: 2),
                        CreatedByGuideId = c.Int(),
                        ApprovalStatus = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedByGuideId)
                .Index(t => t.CreatedByGuideId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false, maxLength: 254),
                        Phone = c.String(nullable: false, maxLength: 20),
                        PasswordHash = c.String(nullable: false, maxLength: 255),
                        FullName = c.String(nullable: false, maxLength: 100),
                        Role = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Email, unique: true, name: "UX_Users_Email")
                .Index(t => t.Phone, unique: true, name: "UX_Users_Phone");
            
            CreateTable(
                "dbo.GuideAvailabilities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GuideId = c.Int(nullable: false),
                        DayOfWeek = c.Int(nullable: false),
                        DayName = c.String(nullable: false, maxLength: 20),
                        IsAvailable = c.Boolean(nullable: false),
                        TimeSlot = c.String(maxLength: 50),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.GuideId)
                .Index(t => t.GuideId);
            
            CreateTable(
                "dbo.Hotels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DestinationId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 150),
                        Address = c.String(nullable: false, maxLength: 300),
                        Description = c.String(maxLength: 2000),
                        PricePerNight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Rating = c.Int(nullable: false),
                        ImageUrl = c.String(maxLength: 500),
                        CreatedByGuideId = c.Int(),
                        ApprovalStatus = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedByGuideId)
                .ForeignKey("dbo.Destinations", t => t.DestinationId)
                .Index(t => t.DestinationId)
                .Index(t => t.CreatedByGuideId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bookings", "UserId", "dbo.Users");
            DropForeignKey("dbo.Bookings", "HotelId", "dbo.Hotels");
            DropForeignKey("dbo.Bookings", "GuideId", "dbo.Users");
            DropForeignKey("dbo.Bookings", "DestinationId", "dbo.Destinations");
            DropForeignKey("dbo.Destinations", "CreatedByGuideId", "dbo.Users");
            DropForeignKey("dbo.Hotels", "DestinationId", "dbo.Destinations");
            DropForeignKey("dbo.Hotels", "CreatedByGuideId", "dbo.Users");
            DropForeignKey("dbo.GuideAvailabilities", "GuideId", "dbo.Users");
            DropIndex("dbo.Hotels", new[] { "CreatedByGuideId" });
            DropIndex("dbo.Hotels", new[] { "DestinationId" });
            DropIndex("dbo.GuideAvailabilities", new[] { "GuideId" });
            DropIndex("dbo.Users", "UX_Users_Phone");
            DropIndex("dbo.Users", "UX_Users_Email");
            DropIndex("dbo.Destinations", new[] { "CreatedByGuideId" });
            DropIndex("dbo.Bookings", "UX_Bookings_BookingId");
            DropIndex("dbo.Bookings", new[] { "DestinationId" });
            DropIndex("dbo.Bookings", new[] { "HotelId" });
            DropIndex("dbo.Bookings", new[] { "GuideId" });
            DropIndex("dbo.Bookings", new[] { "UserId" });
            DropTable("dbo.Hotels");
            DropTable("dbo.GuideAvailabilities");
            DropTable("dbo.Users");
            DropTable("dbo.Destinations");
            DropTable("dbo.Bookings");
        }
    }
}
