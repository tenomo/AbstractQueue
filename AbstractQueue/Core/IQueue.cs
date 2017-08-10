using AbstractQueue.QueueData.Entities;
namespace AbstractQueue.Core
{
    internal interface IQueue   :   ITaskExecutionObserve
    {
        int AttemptMaxCount { get; }
        string QueueName { get; set; }
        int AddTask(QueueTask queueTask);

    }
}