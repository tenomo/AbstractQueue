using System.Data.Entity;

namespace AbstractQueue.QueueData
{
    class QueueDataBaseContext : DbContext
    {
        /// <summary>
        /// Queue task set.
        /// </summary>
        public DbSet<QueueTask> QueueTask { get; set; }
        public static string ConnectionName { get; set; } = "DefaultConnection";


        public QueueDataBaseContext():base(ConnectionName)
        { 
            var dbInitializerStrategy = new CreateDatabaseIfNotExists<QueueDataBaseContext>();
            Database.SetInitializer(new CreateDatabaseIfNotExists<QueueDataBaseContext>());

        }
         
        public QueueDataBaseContext(string connectionName) : base(connectionName)
        {
            var dbInitializerStrategy = new CreateDatabaseIfNotExists<QueueDataBaseContext>();
            Database.SetInitializer(new CreateDatabaseIfNotExists<QueueDataBaseContext>());

        }
         
    }
}
