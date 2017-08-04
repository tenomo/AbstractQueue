namespace AbstractQueue
{
    class QueueFactory
    {
        /// <summary>
        /// Create Queue which the try handle failed task n times.
        /// </summary>
        /// <param name="threadCount"></param>
        /// <param name="executer"></param>
        /// <param name="isHandleFailed"></param>
        /// <param name="countHandleFailed"></param>
        /// <returns></returns>
        public static Queue CreateQueueHandleFailed(int threadCount, AbstractTaskExecuter executer,
            int countHandleFailed, IQueueDBContext queueDbContext)
        {
            return new Queue(threadCount, executer, queueDbContext, countHandleFailed);
        }

        /// <summary>
        /// Create Queue.
        /// </summary>
        /// <param name="threadCount"></param>
        /// <param name="executer"></param>
        /// <returns></returns>
        public static Queue CreateQueue(int threadCount, AbstractTaskExecuter executer, IQueueDBContext queueDbContext)
        {
            return new Queue(threadCount, executer, queueDbContext);
        }
    }
}
