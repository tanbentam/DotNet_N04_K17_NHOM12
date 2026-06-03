using System;
using System.Data.Entity.Migrations;

namespace TravelApp.Backend.Migrations
{
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(maxLength: 255, unicode: false),
                        Phone = c.String(maxLength: 20, unicode: false),
                        PasswordHash = c.String(maxLength: 512, unicode: false),
                        FullName = c.String(maxLength: 255, unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Email, unique: true, name: "IX_User_Email");

            CreateTable(
                "dbo.Bookings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        GuideId = c.Int(nullable: false),
                        HotelId = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false, precision: 0),
                        Nights = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Status = c.String(maxLength: 50, unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: false)
                .Index(t => t.UserId)
                .Index(t => t.GuideId)
                .Index(t => t.HotelId);
        }

        public override void Down()
        {
            DropForeignKey("dbo.Bookings", "UserId", "dbo.Users");
            DropIndex("dbo.Bookings", new[] { "HotelId" });
            DropIndex("dbo.Bookings", new[] { "GuideId" });
            DropIndex("dbo.Bookings", new[] { "UserId" });
            DropIndex("dbo.Users", "IX_User_Email");
            DropTable("dbo.Bookings");
            DropTable("dbo.Users");
        }
    }
}
