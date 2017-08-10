using System;
using AbstractQueue.QueueData.Entities;
namespace AbstractQueue.Infrastructure
{
    public interface ITaskExecutionObserve
    {
        event Action<QueueTask> SuccessExecuteTaskEvent;
        event Action<QueueTask> FailedExecuteTaskEvent;
        event Action<QueueTask> InProccesTaskEvent;
    }
}
