namespace AbstractQueueUnitTests.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fix : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.QueueTasks", "CreationDate", c => c.DateTime());
            AlterColumn("dbo.QueueTasks", "ExecutedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.QueueTasks", "ExecutedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.QueueTasks", "CreationDate", c => c.DateTime(nullable: false));
        }
    }
}
