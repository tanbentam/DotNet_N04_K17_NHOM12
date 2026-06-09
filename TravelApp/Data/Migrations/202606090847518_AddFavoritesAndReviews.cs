namespace TravelApp.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFavoritesAndReviews : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Favorites",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        HotelId = c.Int(),
                        GuideId = c.Int(),
                        CreatedAt = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.GuideId)
                .ForeignKey("dbo.Hotels", t => t.HotelId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.HotelId)
                .Index(t => t.GuideId);
            
            CreateTable(
                "dbo.Reviews",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        HotelId = c.Int(),
                        GuideId = c.Int(),
                        Rating = c.Int(nullable: false),
                        Comment = c.String(maxLength: 1000, storeType: "nvarchar"),
                        CreatedAt = c.DateTime(nullable: false, precision: 0),
                        UpdatedAt = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.GuideId)
                .ForeignKey("dbo.Hotels", t => t.HotelId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.HotelId)
                .Index(t => t.GuideId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Reviews", "UserId", "dbo.Users");
            DropForeignKey("dbo.Reviews", "HotelId", "dbo.Hotels");
            DropForeignKey("dbo.Reviews", "GuideId", "dbo.Users");
            DropForeignKey("dbo.Favorites", "UserId", "dbo.Users");
            DropForeignKey("dbo.Favorites", "HotelId", "dbo.Hotels");
            DropForeignKey("dbo.Favorites", "GuideId", "dbo.Users");
            DropIndex("dbo.Reviews", new[] { "GuideId" });
            DropIndex("dbo.Reviews", new[] { "HotelId" });
            DropIndex("dbo.Reviews", new[] { "UserId" });
            DropIndex("dbo.Favorites", new[] { "GuideId" });
            DropIndex("dbo.Favorites", new[] { "HotelId" });
            DropIndex("dbo.Favorites", new[] { "UserId" });
            DropTable("dbo.Reviews");
            DropTable("dbo.Favorites");
        }
    }
}
