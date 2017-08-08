namespace AbstractQueueUnitTests.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fix2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.QueueTasks", "TaskIdInQueue", c => c.Int(nullable: false));
            DropColumn("dbo.QueueTasks", "IndexInQueue");
        }
        
        public override void Down()
        {
            AddColumn("dbo.QueueTasks", "IndexInQueue", c => c.Int(nullable: false));
            DropColumn("dbo.QueueTasks", "TaskIdInQueue");
        }
    }
}
