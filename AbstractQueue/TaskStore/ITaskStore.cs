using System;
using AbstractQueue.QueueData;

namespace AbstractQueue.TaskStore
{
    public interface ITaskStore : SaveChanges, IExecutedTask
    {
        void SetFailed(QueueTask task);
        void SetSuccess(QueueTask task); 
    }
}