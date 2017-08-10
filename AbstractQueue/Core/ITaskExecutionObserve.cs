using System;
using AbstractQueue.QueueData.Entities;
namespace AbstractQueue.Core
{
    public interface ITaskExecutionObserve
    {
        event Action<QueueTask> SuccessExecuteTaskEvent;
        event Action<QueueTask> FailedExecuteTaskEvent;
        event Action<QueueTask> InProccesTaskEvent;
    }
}
