using System;
using AbstractQueue.QueueData.Entities;

namespace AbstractQueueUnitTests.Mock
{
    internal class MockBehaviorErorrTaskExecution : MockBehaviorTaskExecution
    {
        public override void Execute(QueueTask queueTask)
        {
            if (queueTask.Attempt < 2)
                throw new ExecutionEngineException();
            base.Execute(queueTask);
        }
    }
}
