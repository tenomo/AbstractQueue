using AbstractQueue.QueueData.Entities;
namespace AbstractQueue.Core
{
    public  interface IQueue   :   ITaskExecutionObserve
    {
        int AttemptMaxCount { get; }
        string QueueName { get; set; }
        int AddTask(QueueTask queueTask);

    }
}