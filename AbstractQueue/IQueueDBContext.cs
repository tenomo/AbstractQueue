using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace AbstractQueue
{
    public interface IQueueDBContext  : SaveChanges
    {
        DbSet<QueueTask> Tasks { get; set; }
   
    }

    public interface SaveChanges
    {
        int SaveChanges();
    }
}