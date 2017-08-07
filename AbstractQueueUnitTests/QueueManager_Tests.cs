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


        private readonly string[] queue_names;


        public QueueManager_Tests()
        {
            QueueDbContext = new QueueDBContext();
            queue_names =  new  [] { QueueNameOne, QueueNameTwo, QueueNameThree };
        }
        [TestMethod ]
 
        public void Test_Get_Queue()
        {
            
          var queue_one =   QueueFactory.CreateQueue(2, new MessageExecuter(), QueueDbContext, QueueNameOne);
            var queue_two= QueueFactory.CreateQueue(3, new MessageExecuter(), QueueDbContext, QueueNameTwo);
            var queue_three = QueueFactory.CreateQueue(4, new MessageExecuter(), QueueDbContext, QueueNameThree);


            QueueManager.Kernel.RegistrateQueue(queue_one);
            QueueManager.Kernel.RegistrateQueue(queue_two);
        QueueManager.Kernel.RegistrateQueue(queue_three);


            Assert.AreEqual(queue_one, QueueManager.Kernel[QueueNameOne]);
            Assert.AreEqual(queue_two, QueueManager.Kernel[QueueNameTwo]);
            Assert.AreEqual(queue_three, QueueManager.Kernel[QueueNameThree]);
          
        }

        [TestMethod]
     
        public void Test_Delete_Queue()
        {
            int queue_s_Count = QueueManager.Kernel.QueuesCount;
            System.Diagnostics.Debug.WriteLine(queue_s_Count);
            foreach (var VARIABLE in queue_names)
            {
                       QueueManager.Kernel.DeleteQueue(VARIABLE); 
            }
     

            int ideal = queue_s_Count - 3;
            System.Diagnostics.Debug.WriteLine(ideal);
            Assert.AreEqual(QueueManager.Kernel.QueuesCount , ideal);

        }
    }
}
 
