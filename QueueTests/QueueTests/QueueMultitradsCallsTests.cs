using System;
using System.Threading;
using System.Threading.Tasks;
using AbstractQueue.Core;
using AbstractQueue.QueueData.Entities;
using AbstractQueueUnitTests.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AbstractQueueUnitTests.QueueTests
{
    [TestClass]
    public class QueueMultitradsCallsTests
    {
        private void WaitTast(int timeout)
        {
            Thread.Sleep(timeout);
        }
        Random rnd = new Random();

        [TestMethod]
        public void execute_task_1_worker_self_tread__10_Iterations()
        {
            int itterationCount = 10;
            const string queueName = "execute_task_1_worker_self_tread__10_Iterations";
            int workerscount = 4;
            var executer = new MockBehaviorTaskExecution();
            var queue = QueueFactory.CreateQueue(workerscount, executer, queueName);

            for (int i = 1; i <= itterationCount; i++)
            {
                new TaskFactory().StartNew(() =>
                {
                    queue.AddTask(QueueTask.Create(0, i.ToString()));
                });
                var a = rnd.Next(1, 31);
                if (a % 2 == 0)
                {
                    WaitTast(100);
                }
            }
            WaitTast(10000);
            Assert.AreEqual(itterationCount.ToString(), executer.ExecutionTaskCount.ToString());
        }

        [TestMethod]
        public void execute_task_4_worker_self_tread__10_Iterations()
        {
            int itterationCount = 10;
            const string queueName = "execute_task_4_worker_self_tread__10_Iterations";
            int workerscount = 4;
            var executer = new MockBehaviorTaskExecution();
            var queue = QueueFactory.CreateQueue(workerscount, executer, queueName);

            for (int i = 1; i <= itterationCount; i++)
            {
                new TaskFactory().StartNew(() =>
                {
                    queue.AddTask(QueueTask.Create(0, i.ToString()));
                });
                var a = rnd.Next(1, 31);
                if (a % 2 == 0)
                {
                    WaitTast(100);
                }
            }
            WaitTast(10000);
            Assert.AreEqual(itterationCount.ToString(), executer.ExecutionTaskCount.ToString());
        }

        [TestMethod]
        public void execute_task_1_worker_self_tread__100_Iterations()
        {
            int itterationCount = 100;
            const string queueName = "execute_task_1_worker_self_tread__100_Iterations";
            int workerscount = 4;
            var executer = new MockBehaviorTaskExecution();
            var queue = QueueFactory.CreateQueue(workerscount, executer, queueName);

            for (int i = 1; i <= itterationCount; i++)
            {
                new TaskFactory().StartNew(() =>
                {
                    queue.AddTask(QueueTask.Create(0, i.ToString()));
                });
                var a = rnd.Next(1, 31);
                if (a % 2 == 0)
                {
                    WaitTast(100);
                }
            }
            WaitTast(100);
            Assert.AreEqual(itterationCount.ToString(), executer.ExecutionTaskCount.ToString());
        }

        [TestMethod]
        public void execute_task_4_worker_self_tread__100_Iterations()
        {
            int itterationCount = 100;
            const string queueName = "execute_task_4_worker_self_tread__100_Iterations";
            int workerscount = 4;
            var executer = new MockBehaviorTaskExecution();
            var queue = QueueFactory.CreateQueue(workerscount, executer, queueName);

            for (int i = 1; i <= itterationCount; i++)
            {
                new TaskFactory().StartNew(() =>
                {
                    queue.AddTask(QueueTask.Create(0, i.ToString()));
                });
                var a = rnd.Next(1, 31);
                if (a % 2 == 0)
                {
                    WaitTast(100);
                }
            }
            WaitTast(1 - 0);
            Assert.AreEqual(itterationCount.ToString(), executer.ExecutionTaskCount.ToString());
        }
    }
}
