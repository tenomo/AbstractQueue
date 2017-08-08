namespace AbstractQueueUnitTests.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fix1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.QueueTasks", "IndexInQueue", c => c.Int(nullable: false));
            DropColumn("dbo.QueueTasks", "TaskIdInQueue");
        }
        
        public override void Down()
        {
            AddColumn("dbo.QueueTasks", "TaskIdInQueue", c => c.Int(nullable: false));
            DropColumn("dbo.QueueTasks", "IndexInQueue");
        }
    }
}
