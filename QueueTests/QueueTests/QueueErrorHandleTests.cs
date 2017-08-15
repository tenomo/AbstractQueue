using System;
using System.Threading;
using AbstractQueue;
using AbstractQueue.Core;
using AbstractQueue.QueueData.Entities;
using AbstractQueueUnitTests.Mock;
using NUnit.Framework;


namespace AbstractQueueUnitTests.QueueTests
{
    [TestFixture]
    public class QueueErrorHandleTests
    {
        private void WaitTast(int timeout)
        {
            Thread.Sleep(timeout);
        }

        public QueueErrorHandleTests()
        {
            Config.IsConnectionName = false;
            Config.ConnectionStringName =
                System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        Random rnd = new Random();

        #region error handle
        [Test]
        public void execute_tasks_ERROR_HANDLE_ATTEMPS_1_worker_10_Iterations()
        {
            int itterationCount = 10;
            const string queueName = "execute_tasks_ERROR_HANDLE_ATTEMPS_1_worker_10_Iterations";
            int workerscount = 1;
            var executer = new MockBehaviorErorrTaskExecution();
            var queue = QueueFactory.CreateQueueHandleFailed(workerscount, executer, 3, queueName);

            for (int i = 1; i <= itterationCount; i++)
            {
                queue.AddTask(QueueTask.Create(0, i.ToString()));
            }
            WaitTast(10000);
            Assert.AreEqual(itterationCount.ToString(), executer.ExecutionTaskCount.ToString());
        }

        [Test]
        public void execute_tasks_ERROR_HANDLE_ATTEMPS_4_worker_10_Iterations()
        {
            int itterationCount = 10;
            const string queueName = "execute_tasks_ERROR_HANDLE_ATTEMPS_4_worker_10_Iterations";
            int workerscount = 4;

            var executer = new MockBehaviorErorrTaskExecution();

            var queue = QueueFactory.CreateQueueHandleFailed(workerscount, executer, 3, queueName);

            for (int i = 1; i <= itterationCount; i++)
            {
                queue.AddTask(QueueTask.Create(0, i.ToString()));
            }
            WaitTast(30000);
            Assert.AreEqual(itterationCount.ToString(), executer.ExecutionTaskCount.ToString());
        }




        [Test]
        public void execute_task_ERROR_HANDLE_ATTEMPS_1_worker_1_self_tread__10_Iterations()
        {
            int itterationCount = 10;
            const string queueName = "execute_task_ERROR_HANDLE_ATTEMPS_1_worker_1_self_tread__10_Iterations";
            int workerscount = 4;
            var executer = new MockBehaviorErorrTaskExecution();

            var queue = QueueFactory.CreateQueueHandleFailed(workerscount, executer, 3, queueName);

            for (int i = 1; i <= itterationCount; i++)
            {
                 
                    queue.AddTaskAsync(QueueTask.Create(0, i.ToString())).Wait();
                
                var a = rnd.Next(1, 31);
                if (a % 2 == 0)
                {
                    WaitTast(2);
                }
            }
       
            Assert.AreEqual(itterationCount.ToString(), executer.ExecutionTaskCount.ToString());
        }

        [Test]
        public void execute_task_ERROR_HANDLE_ATTEMPS_1_worker_4_self_tread__10_Iterations()
        {
            int itterationCount = 10;
            const string queueName = "execute_task_ERROR_HANDLE_ATTEMPS_1_worker_4_self_tread__10_Iterations";
            int workerscount = 4;
            var executer = new MockBehaviorErorrTaskExecution();

            var queue = QueueFactory.CreateQueueHandleFailed(workerscount, executer, 3, queueName);

            for (int i = 1; i <= itterationCount; i++)
            {
                
                    queue.AddTaskAsync(QueueTask.Create(0, i.ToString())).Wait();
                 
                var a = rnd.Next(1, 31);
                if (a % 2 == 0)
                {
                    WaitTast(2);
                }
            }
         
            Assert.AreEqual(itterationCount.ToString(), executer.ExecutionTaskCount.ToString());
        }

        #endregion
    }
}
