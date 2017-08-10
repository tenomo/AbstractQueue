using System;
using AbstractQueue.QueueData.Entities;

namespace AbstractQueue.Infrastructure
{
  internal  class TaskExecutionObserver: Singleton<TaskExecutionObserver>,ITaskExecutionObserve
    {
        public event Action<QueueTask> SuccessExecuteTaskEvent;
        public event Action<QueueTask> FailedExecuteTaskEvent;
        public event Action<QueueTask> InProccesTaskEvent;


        internal void OnSuccessExecuteTaskEvent(QueueTask obj)
        {
            SuccessExecuteTaskEvent?.Invoke(obj);
        }

        internal virtual void OnFailedExecuteTaskEvent(QueueTask obj)
        {
            FailedExecuteTaskEvent?.Invoke(obj);
        }

        internal virtual void OnInProccesTaskEvent(QueueTask obj)
        {
            InProccesTaskEvent?.Invoke(obj);
        }
    }
}
