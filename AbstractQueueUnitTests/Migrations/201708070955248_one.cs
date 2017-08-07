namespace AbstractQueueUnitTests.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class one : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.QueueTasks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TaskIdInQueue = c.Int(nullable: false),
                        QueueName = c.String(),
                        QueueTaskStatus = c.Int(nullable: false),
                        Type = c.Byte(nullable: false),
                        Body = c.String(),
                        Attempt = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.QueueTasks");
        }
    }
}
