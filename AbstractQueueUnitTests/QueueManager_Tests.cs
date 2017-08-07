using System;
using System.Data.Entity;
using AbstractQueue;
using AbstractQueueUnitTests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AbstractQueueUnitTests
{
    [TestClass]
    public class QueueManager_Tests
    {
        private readonly QueueDBContext QueueDbContext;
        private readonly string QueueNameOne = "One";
        private readonly string QueueNameTwo = "Two";
        private readonly string QueueNameThree = "Three";
        public QueueManager_Tests()
        {
            QueueDbContext = new QueueDBContext();
            
        }
        [TestMethod]
        public void TestGetQueue()
        {
          var queue_one =   QueueFactory.CreateQueue(5, new MessageExecuter(), QueueDbContext, QueueNameOne);
            var queue_two= QueueFactory.CreateQueue(5, new MessageExecuter(), QueueDbContext, QueueNameTwo);
            var queue_three = QueueFactory.CreateQueue(5, new MessageExecuter(), QueueDbContext, QueueNameThree);


            QueueManager.Kernel.RegistrateQueue(queue_one);
            QueueManager.Kernel.RegistrateQueue(queue_two);
        QueueManager.Kernel.RegistrateQueue(queue_three);


            Assert.AreEqual(queue_one, QueueManager.Kernel[QueueNameOne]);
            Assert.AreEqual(queue_two, QueueManager.Kernel[QueueNameTwo]);
            Assert.AreEqual(queue_three, QueueManager.Kernel[QueueNameThree]);

        } 
    }
}
 
