namespace TravelApp.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddContentApproval : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Destinations", "CreatedByGuideId", c => c.Int());
            AddColumn(
                "dbo.Destinations",
                "ApprovalStatus",
                c => c.Int(nullable: false, defaultValue: 1));
            AddColumn("dbo.Hotels", "CreatedByGuideId", c => c.Int());
            AddColumn(
                "dbo.Hotels",
                "ApprovalStatus",
                c => c.Int(nullable: false, defaultValue: 1));
            Sql("UPDATE Destinations SET ApprovalStatus = 1 WHERE ApprovalStatus = 0");
            Sql("UPDATE Hotels SET ApprovalStatus = 1 WHERE ApprovalStatus = 0");
            CreateIndex("dbo.Destinations", "CreatedByGuideId");
            CreateIndex("dbo.Hotels", "CreatedByGuideId");
            AddForeignKey("dbo.Hotels", "CreatedByGuideId", "dbo.Users", "Id");
            AddForeignKey("dbo.Destinations", "CreatedByGuideId", "dbo.Users", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Destinations", "CreatedByGuideId", "dbo.Users");
            DropForeignKey("dbo.Hotels", "CreatedByGuideId", "dbo.Users");
            DropIndex("dbo.Hotels", new[] { "CreatedByGuideId" });
            DropIndex("dbo.Destinations", new[] { "CreatedByGuideId" });
            DropColumn("dbo.Hotels", "ApprovalStatus");
            DropColumn("dbo.Hotels", "CreatedByGuideId");
            DropColumn("dbo.Destinations", "ApprovalStatus");
            DropColumn("dbo.Destinations", "CreatedByGuideId");
        }
    }
}
