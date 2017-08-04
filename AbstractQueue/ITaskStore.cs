using System;

namespace AbstractQueue
{
    public interface ITaskStore : SaveChanges, IExecutedTask
    {
        void SetFailed(QueueTask task);
        void SetSuccess(QueueTask task); 
    }
}