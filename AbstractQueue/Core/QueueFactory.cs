namespace AbstractQueue.Core
{
  public static class QueueFactory
    {
        /// <summary>
        /// Create Queue which the try handle failed task n times.
        /// </summary>
        /// <param name="threadCount"></param>
        /// <param name="executer"></param>
        /// <param name="isHandleFailed"></param>
        /// <param name="countHandleFailed"></param>
        /// <returns></returns>
        public static IQueue CreateQueueHandleFailed(int threadCount, AbstractTaskExecuter executer,int countHandleFailed ,string queueName)
        {
            return new Queue(threadCount, executer,  queueName, countHandleFailed);
        }

        /// <summary>
        /// Create Queue.
        /// </summary>
        /// <param name="threadCount"></param>
        /// <param name="executer"></param>
        /// <returns></returns>
        public static IQueue CreateQueue(int threadCount, AbstractTaskExecuter executer, string queueName)
        {
            return new Queue(threadCount, executer,  queueName);
        }
    }
}
