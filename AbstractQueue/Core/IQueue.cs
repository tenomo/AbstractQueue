using AbstractQueue.Infrastructure;
using AbstractQueue.QueueData.Entities;
namespace AbstractQueue.Core
{
    public  interface IQueue  // :   ITaskExecutionObserver
    {
        int AttemptMaxCount { get; }
        string QueueName { get; set; }
        int AddTask(QueueTask queueTask);
        ITaskExecutionObserver TaskExecutionEvents { get; }

    }
}