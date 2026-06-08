namespace TravelApp.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddGuideAvailabilities : DbMigration
    {
        public override void Up()
        {
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
                        UpdatedAt = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.GuideId)
                .Index(t => t.GuideId);
        }

        public override void Down()
        {
            DropForeignKey("dbo.GuideAvailabilities", "GuideId", "dbo.Users");
            DropIndex("dbo.GuideAvailabilities", new[] { "GuideId" });
            DropTable("dbo.GuideAvailabilities");
        }
    }
}
