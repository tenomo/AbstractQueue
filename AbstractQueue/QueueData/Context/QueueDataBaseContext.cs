using System.Data.Entity;
using AbstractQueue.QueueData.Entities;

namespace AbstractQueue.QueueData.Context
{
   internal class QueueDataBaseContext : DbContext
    {
        /// <summary>
        /// Queue task set.
        /// </summary>
        public DbSet<QueueTask> QueueTasks { get; set; } 

        private readonly CreateDatabaseIfNotExists<QueueDataBaseContext> dbInitializerStrategy;

        internal QueueDataBaseContext():base("name=DefaultConnection")
        { 
            dbInitializerStrategy = new CreateDatabaseIfNotExists<QueueDataBaseContext>();
            Database.SetInitializer(dbInitializerStrategy);

        }

        internal QueueDataBaseContext(string connectionString, bool isConnectionName = true) : base(isConnectionName? "name=" + connectionString : connectionString)
        { 
            dbInitializerStrategy = new CreateDatabaseIfNotExists<QueueDataBaseContext>();
            Database.SetInitializer(dbInitializerStrategy);
        }


      
    }


}
