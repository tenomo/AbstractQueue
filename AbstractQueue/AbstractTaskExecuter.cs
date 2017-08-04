namespace AbstractQueue
{

    public abstract class AbstractTaskExecuter
    {
        public ITaskStore TaskStore { get; internal set; }

        /// <summary>
        /// Execute queueTask.
        /// </summary>
        /// <param name="queueTask"></param>
        public abstract void Execute(QueueTask queueTask);
 
    }
}
