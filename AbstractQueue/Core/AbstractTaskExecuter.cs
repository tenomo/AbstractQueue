using AbstractQueue.QueueData.Entities;

namespace AbstractQueue.Core
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
