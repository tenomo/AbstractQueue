using System.Data.Entity;

namespace AbstractQueue.QueueData
{
   internal class QueueDataBaseContext : DbContext
    {
        /// <summary>
        /// Queue task set.
        /// </summary>
        public DbSet<QueueTask> QueueTasks { get; set; }
        public static string ConnectionName { get; set; } = "DefaultConnection";

        private readonly CreateDatabaseIfNotExists <QueueDataBaseContext> dbInitializerStrategy = new CreateDatabaseIfNotExists<QueueDataBaseContext>();

        public QueueDataBaseContext():base(ConnectionName)
        {
            Database.SetInitializer(dbInitializerStrategy);

        }
         
        public QueueDataBaseContext(string connectionName) : base(connectionName)
        {
            Database.SetInitializer(dbInitializerStrategy);

        }

         
    }
}
