using AbstractQueue.Core;
using AbstractQueue.QueueData.Entities;

namespace AbstractQueueUnitTests.Mock
{
   internal class MockBehaviorTaskExecution : BehaviorTaskExecution
    {
        private int _executionTaskCount = 0;
        private object lockObj = new object();

        public int ExecutionTaskCount
        {
            get
            {
                lock (lockObj)
                {
                    return _executionTaskCount;
                }
            }
            set
            {
                lock (lockObj)
                {
                    _executionTaskCount = value;

                }

            }
        }

        public override void Execute(QueueTask queueTask)
        {
            ExecutionTaskCount++;
        }
    }
}
