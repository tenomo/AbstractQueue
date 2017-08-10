using System;
using AbstractQueue.QueueData.Entities;
using AbstractQueue.TaskStore;

namespace AbstractQueue.Infrastructure
{
    public interface ITaskExecutionObserver
    {
        event Action<ITaskStore, QueueTask> SuccessExecuteTaskEvent;
        event Action<ITaskStore, QueueTask> FailedExecuteTaskEvent;
        event Action<ITaskStore, QueueTask> InProccesTaskEvent;
    }
}
