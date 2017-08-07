namespace AbstractQueueUnitTests.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.QueueTasks", "CreationDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.QueueTasks", "ExecutedDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.QueueTasks", "ExecutedDate");
            DropColumn("dbo.QueueTasks", "CreationDate");
        }
    }
}
