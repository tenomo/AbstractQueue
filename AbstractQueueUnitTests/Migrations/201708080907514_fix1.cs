namespace AbstractQueueUnitTests.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fix1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.QueueTasks", "IndexInQueue", c => c.Int(nullable: false));
            DropColumn("dbo.QueueTasks", "TaskIndexInQueue");
        }
        
        public override void Down()
        {
            AddColumn("dbo.QueueTasks", "TaskIndexInQueue", c => c.Int(nullable: false));
            DropColumn("dbo.QueueTasks", "IndexInQueue");
        }
    }
}
