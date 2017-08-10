using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AbstractQueue;
using AbstractQueue.Core;
using AbstractQueue.QueueData.Entities; 
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;

namespace AbstractQueueUnitTests
{
    [TestClass]
    public class QueueManager_Tests
    {
        
        private readonly string QueueNameOne = "One";
        private readonly string QueueNameTwo = "Two";
        private readonly string QueueNameThree = "Three";

      //  private readonly QueueDBContext context;
        private readonly string[] queue_names;


        public QueueManager_Tests()
        {
       
            queue_names =  new  [] { QueueNameOne, QueueNameTwo, QueueNameThree };
        //    context =DBSingle.DbContext;
        }
        [TestMethod]

        public void Test_Get_Queue()
        {
           // var context = new QueueDBContext();
            var queue_one = QueueFactory.CreateQueue(2, new MockTaskExecuter(), QueueNameOne);
            var queue_two = QueueFactory.CreateQueue(3, new MockTaskExecuter(),   QueueNameTwo);
            var queue_three = QueueFactory.CreateQueue(4, new MockTaskExecuter(),   QueueNameThree);


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
            Assert.AreEqual(QueueManager.Kernel.QueuesCount, ideal);

        }

        /// <summary>
        /// Test queue with QueueManager on NumberCalculateExecuter with two workers
        /// </summary>
        [TestMethod]
        public void Test_Queue_QM_On_NumberCalculateExecuter_with_2_Workers()
        { 
            int itterationCount = 10;
            int executedTasksCount = 1;
            const string queueName = "Test_Queue_QM_On_NumberCalculateExecuter_with_2_Workers";
            var queue = QueueFactory.CreateQueue(2, new MockTaskExecuter(), queueName);
            queue.SuccessExecuteTaskEvent += delegate(QueueTask task)
            {
                executedTasksCount++;
            };
            QueueManager.Kernel.RegistrateQueue(queue);

            for (int i = 0; i < itterationCount; i++)
            {
                QueueManager.Kernel[queueName].AddTask(QueueTask.Create(0,i.ToString()));
            }
            Assert.AreEqual(itterationCount, executedTasksCount);
        }


        [TestMethod]
        public void Test_Queue_QM__with_2_Workers_multy_thread_calls()
        {
            int itterationCount = 10;
            int executedTasksCount = 1;
            const string queueName = "Test_Queue_QM_On_NumberCalculateExecuter_with_2_Workers";
            var queue = QueueFactory.CreateQueue(2, new MockTaskExecuter(), queueName);
            queue.SuccessExecuteTaskEvent += delegate (QueueTask task)
            {
                executedTasksCount++;
            };
            QueueManager.Kernel.RegistrateQueue(queue);

            for (int i = 0; i < itterationCount; i++)
            {
                new TaskFactory().StartNew(() => { QueueManager.Kernel[queueName].AddTask(QueueTask.Create(0, i.ToString())); });
      
            }
            Assert.AreEqual(itterationCount, executedTasksCount);
        }

        class MockTaskExecuter :AbstractTaskExecuter
        {
            public override void Execute(QueueTask queueTask)
            {
                 
            }
        }

    }
    }
 
 
