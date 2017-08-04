using System;

namespace AbstractQueue
{
    public interface IQueue
    {
        int CountHandleFailed { get; }
        string QueueName { get; set; }

        event Action<QueueTask> ExecutedTask;

        int AddTask(QueueTask queueTask);
    }
}