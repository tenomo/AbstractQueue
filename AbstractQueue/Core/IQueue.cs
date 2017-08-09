using System;
using AbstractQueue.QueueData.Entities;

namespace AbstractQueue.Core
{
    public interface IQueue
    {
        int CountHandleFailed { get; }
        string QueueName { get; set; }

        event Action<QueueTask> ExecutedTaskEvent;

        int AddTask(QueueTask queueTask);
    }
}