using System;
using AbstractQueue.QueueData.Entities;
using AbstractQueue.TaskStore;

namespace AbstractQueue.Infrastructure
{
  internal  class TaskExecutionObserver: Singleton<TaskExecutionObserver>,ITaskExecutionObserver
    {
        public event Action<ITaskStore, QueueTask  > SuccessExecuteTaskEvent;
        public event Action<ITaskStore, QueueTask> FailedExecuteTaskEvent;
        public event Action<ITaskStore, QueueTask> InProccesTaskEvent;


        internal void OnSuccessExecuteTaskEvent(ITaskStore obj , QueueTask e )
        {
            SuccessExecuteTaskEvent?.Invoke(obj,e);
        }

        internal virtual void OnFailedExecuteTaskEvent(ITaskStore obj, QueueTask e)
        {
            FailedExecuteTaskEvent?.Invoke(obj, e);
        }

        internal virtual void OnInProccesTaskEvent(ITaskStore obj, QueueTask e)
        {
            InProccesTaskEvent?.Invoke(obj, e);
        }
    }
}
