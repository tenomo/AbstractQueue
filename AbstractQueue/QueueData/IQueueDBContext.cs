using System.Data.Entity;

namespace AbstractQueue.QueueData
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