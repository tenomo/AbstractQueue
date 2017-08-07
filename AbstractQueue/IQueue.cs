using System;

namespace AbstractQueue
{
    public interface IQueue
    {
        int CountHandleFailed { get; }
        string QueueName { get; set; }

        event Action<QueueTask> ExecutedTaskEvent;

        int AddTask(QueueTask queueTask);
    }
}