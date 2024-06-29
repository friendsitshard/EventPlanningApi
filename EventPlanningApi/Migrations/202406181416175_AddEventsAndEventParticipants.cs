namespace EventPlanningApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEventsAndEventParticipants : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EventParticipants",
                c => new
                    {
                        ParticipantId = c.Int(nullable: false, identity: true),
                        EventId = c.Int(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.ParticipantId)
                .ForeignKey("dbo.Events", t => t.EventId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.EventId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        EventId = c.Int(nullable: false, identity: true),
                        EventName = c.String(maxLength: 100),
                        EventCapacity = c.Int(nullable: false),
                        EventDate = c.DateTime(nullable: false),
                        EventLocation = c.String(maxLength: 200),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.EventId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Events", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.EventParticipants", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.EventParticipants", "EventId", "dbo.Events");
            DropIndex("dbo.Events", new[] { "UserId" });
            DropIndex("dbo.EventParticipants", new[] { "UserId" });
            DropIndex("dbo.EventParticipants", new[] { "EventId" });
            DropTable("dbo.Events");
            DropTable("dbo.EventParticipants");
        }
    }
}
