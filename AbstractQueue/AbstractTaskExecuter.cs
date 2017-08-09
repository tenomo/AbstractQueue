using AbstractQueue.TaskStore;

namespace AbstractQueue
{

    public abstract class AbstractTaskExecuter
    {
         

        /// <summary>
        /// Execute queueTask.
        /// </summary>
        /// <param name="queueTask"></param>
        public abstract void Execute(QueueTask queueTask);
 
    }
}
