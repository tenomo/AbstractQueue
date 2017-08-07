using System;
using System.Threading;
using AbstractQueue;
using AbstractQueueUnitTests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AbstractQueueUnitTests
{
    [TestClass]
    public class Queue_Tests
    {
        private QueueDBContext QueueDbContext;
        public Queue_Tests()
        {
            QueueDbContext = new QueueDBContext();
        }
        [TestMethod]
        public void ExecuteTasks_on_1_Worker_Test()
        {
           var queue =  QueueFactory.CreateQueue(1, new MessageExecuter(), QueueDbContext, "ExecuteTasks_on_1_Worker");


            queue.AddTask(new QueueTask((int) MessageTypes.A, " " ));
            queue.AddTask(new QueueTask((int)MessageTypes.B, " "));
            queue.AddTask(new QueueTask((int)MessageTypes.C, " "));
            queue.AddTask(new QueueTask((int)MessageTypes.D, " "));

            Thread.Sleep(15000);

            Assert.AreEqual(MessageState.MessageA,MessageState.MessageA_Ideal);

            Assert.AreEqual(MessageState.MessageB, MessageState.MessageB_Ideal);

            Assert.AreEqual(MessageState.MessageC, MessageState.MessageC_Ideal);

            Assert.AreNotEqual(MessageState.MessageC, MessageState.MessageC_Ideal);
        }
      
    
    }
}
