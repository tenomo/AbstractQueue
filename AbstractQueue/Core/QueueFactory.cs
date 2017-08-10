namespace AbstractQueue.Core
{
  public static class QueueFactory
    {
        /// <summary>
        /// Create Queue which the try handle failed task n times.
        /// </summary>
        /// <param name="workersCount"></param>
        /// <param name="executer"></param>
        /// <param name="isHandleFailed"></param>
        /// <param name="HandleFailedAttempsCount"></param>
        /// <returns></returns>
        public static IQueue CreateQueueHandleFailed(int workersCount, AbstractTaskExecuter executer,int HandleFailedAttempsCount ,string queueName)
        {
            return new Queue(workersCount, executer,  queueName, HandleFailedAttempsCount);
        }

        /// <summary>
        /// Create Queue.
        /// </summary>
        /// <param name="workersCount"></param>
        /// <param name="executer"></param>
        /// <returns></returns>
        public static IQueue CreateQueue(int workersCount, AbstractTaskExecuter executer, string queueName)
        {
            return new Queue(workersCount, executer,  queueName);
        }
    }
}
