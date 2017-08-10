using System;
using System.Runtime.CompilerServices;
using AbstractQueue.QueueData.Entities;
using AbstractQueue.TaskStore;

namespace AbstractQueue.Infrastructure
{    
  public   class TaskExecutionObserver : ITaskExecutionObserver // Singleton<TaskExecutionObserver>,ITaskExecutionObserver
    {
 
      //  [assembly: InternalsVisibleTo("AbstractQueueUnitTests")]
 

        public event Action<ITaskStore, QueueTask  > SuccessExecuteTaskEvent;
        public event Action<ITaskStore, QueueTask> FailedExecuteTaskEvent;
        public event Action<ITaskStore, QueueTask> InProccesTaskEvent;

        

        protected TaskExecutionObserver() { }

        private sealed class TaskExecutionObserverCreator
        {
            private static readonly TaskExecutionObserver instance = new TaskExecutionObserver();
            public static TaskExecutionObserver Instance { get { return instance; } }
        }

        public static TaskExecutionObserver Kernal
        {
            get { return TaskExecutionObserverCreator.Instance; }
        }

        internal void OnSuccessExecuteTaskEvent(ITaskStore obj , QueueTask e )
        {
            SuccessExecuteTaskEvent?.Invoke(obj,e);
        }

        internal void OnFailedExecuteTaskEvent(ITaskStore obj, QueueTask e)
        {
            FailedExecuteTaskEvent?.Invoke(obj, e);
        }

        internal void OnInProccesTaskEvent(ITaskStore obj, QueueTask e)
        {
            InProccesTaskEvent?.Invoke(obj, e);
        }
    }
}
