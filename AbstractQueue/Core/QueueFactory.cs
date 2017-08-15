namespace AbstractQueue.Core
{
  public static class QueueFactory
    {
        /// <summary>
        /// Create Queue which the try handle failed task n times.
        /// </summary>
        /// <param name="workersCount"></param>
        /// <param name="execution></param>
        /// <param name="isHandleFailed"></param>
        /// <param name="HandleFailedAttempsCount"></param>
        /// <returns></returns>
        public static IQueue CreateQueueHandleFailed(int workersCount, BehaviorTaskExecution execution,int HandleFailedAttempsCount ,string queueName)
        {
            return new Queue(workersCount, execution,  queueName, HandleFailedAttempsCount);
        }

        /// <summary>
        /// Create Queue.
        /// </summary>
        /// <param name="workersCount"></param>
        /// <param name="execution></param>
        /// <returns></returns>
        public static IQueue CreateQueue(int workersCount, BehaviorTaskExecution execution, string queueName)
        {
            return new Queue(workersCount, execution,  queueName);
        }
    }
}
