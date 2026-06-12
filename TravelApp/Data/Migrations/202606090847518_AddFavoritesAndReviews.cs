namespace TravelApp.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFavoritesAndReviews : DbMigration
    {
        public override void Up()
        {
            Sql(@"
CREATE TABLE IF NOT EXISTS `Favorites` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `UserId` INT NOT NULL,
    `HotelId` INT NULL,
    `GuideId` INT NULL,
    `CreatedAt` DATETIME NOT NULL,
    PRIMARY KEY (`Id`),
    INDEX `IX_Favorites_UserId` (`UserId`),
    INDEX `IX_Favorites_HotelId` (`HotelId`),
    INDEX `IX_Favorites_GuideId` (`GuideId`),
    CONSTRAINT `FK_Favorites_Users_UserId`
        FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`),
    CONSTRAINT `FK_Favorites_Hotels_HotelId`
        FOREIGN KEY (`HotelId`) REFERENCES `Hotels` (`Id`),
    CONSTRAINT `FK_Favorites_Users_GuideId`
        FOREIGN KEY (`GuideId`) REFERENCES `Users` (`Id`)
) ENGINE=InnoDB;");

            Sql(@"
CREATE TABLE IF NOT EXISTS `Reviews` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `UserId` INT NOT NULL,
    `HotelId` INT NULL,
    `GuideId` INT NULL,
    `Rating` INT NOT NULL,
    `Comment` NVARCHAR(1000) NULL,
    `CreatedAt` DATETIME NOT NULL,
    `UpdatedAt` DATETIME NOT NULL,
    PRIMARY KEY (`Id`),
    INDEX `IX_Reviews_UserId` (`UserId`),
    INDEX `IX_Reviews_HotelId` (`HotelId`),
    INDEX `IX_Reviews_GuideId` (`GuideId`),
    CONSTRAINT `FK_Reviews_Users_UserId`
        FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`),
    CONSTRAINT `FK_Reviews_Hotels_HotelId`
        FOREIGN KEY (`HotelId`) REFERENCES `Hotels` (`Id`),
    CONSTRAINT `FK_Reviews_Users_GuideId`
        FOREIGN KEY (`GuideId`) REFERENCES `Users` (`Id`)
) ENGINE=InnoDB;");
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
