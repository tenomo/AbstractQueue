using System.Data.Entity;
using AbstractQueue.Core;
using AbstractQueue.QueueData.Entities;

namespace AbstractQueue.QueueData.Context
{
   internal class QueueDataBaseContext : DbContext
    {
        /// <summary>
        /// Queue task set.
        /// </summary>
        internal DbSet<QueueTask> QueueTasks { get; set; }
        internal static string ConnectionName { get; set; } = "DefaultConnection";

        private readonly CreateDatabaseIfNotExists <QueueDataBaseContext> dbInitializerStrategy = new CreateDatabaseIfNotExists<QueueDataBaseContext>();

        internal QueueDataBaseContext():base(ConnectionName)
        {
            Database.SetInitializer(dbInitializerStrategy);

        }

        internal QueueDataBaseContext(string connectionName) : base(connectionName)
        {
            Database.SetInitializer(dbInitializerStrategy);

        } 
    }
}
