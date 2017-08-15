using System;
using System.Threading;
using System.Threading.Tasks;
using AbstractQueue;
using AbstractQueue.Core;
using AbstractQueue.QueueData.Entities;
using AbstractQueueUnitTests.Mock; 
using NUnit.Framework;

namespace AbstractQueueUnitTests.QueueTests
{
    [TestFixture]
    public class QueueMultitradsCallsTests
    {

        public QueueMultitradsCallsTests()
        {
            Config.IsConnectionName = false;
            Config.ConnectionStringName =
                System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }
        private void WaitTast(int timeout)
        {
            Thread.Sleep(timeout);
        }
        Random rnd = new Random();

        [Test]
        public void execute_task_1_worker_self_tread__10_Iterations()
        {
            int itterationCount = 10;
            const string queueName = "execute_task_1_worker_self_tread__10_Iterations";
            int workerscount = 4;
            var executer = new MockBehaviorTaskExecution();
            var queue = QueueFactory.CreateQueue(workerscount, executer, queueName);

            for (int i = 1; i <= itterationCount; i++)
            {
               // new TaskFactory().StartNew(() =>
               // {
                    queue.AddTaskAsync(QueueTask.Create(0, i.ToString())).Wait(); 
               // });
                var a = rnd.Next(1, 31);
                if (a % 2 == 0)
                {
                    WaitTast(2);
                }
            }
         //   WaitTast(300);
            Assert.AreEqual(itterationCount.ToString(), executer.ExecutionTaskCount.ToString());
        }

        [Test]
        public void execute_task_4_worker_self_tread__10_Iterations()
        {
            int itterationCount = 10;
            const string queueName = "execute_task_4_worker_self_tread__10_Iterations";
            int workerscount = 4;
            var executer = new MockBehaviorTaskExecution();
            var queue = QueueFactory.CreateQueue(workerscount, executer, queueName);

            for (int i = 1; i <= itterationCount; i++)
            {
               // new TaskFactory().StartNew(() =>
              ///  {
                    queue.AddTaskAsync(QueueTask.Create(0, i.ToString())).Wait(); 
               // }).Wait();
                var a = rnd.Next(1, 31);
                if (a % 2 == 0)
                {
                    WaitTast(100);
                }
            }
       //     WaitTast(300);
            Assert.AreEqual(itterationCount.ToString(), executer.ExecutionTaskCount.ToString());
        }

        [Test]
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
                    queue.AddTaskAsync(QueueTask.Create(0, i.ToString()));
                }).Wait();
                var a = rnd.Next(1, 31);
                if (a % 2 == 0)
                {
                    WaitTast(2);
                }
            }
            WaitTast(1000);
            Assert.AreEqual(itterationCount.ToString(), executer.ExecutionTaskCount.ToString());
        }

        [Test]
        public void execute_task_4_worker_self_tread__100_Iterations()
        {
            int itterationCount = 100;
            const string queueName = "execute_task_4_worker_self_tread__100_Iterations";
            int workerscount = 4;
            var executer = new MockBehaviorTaskExecution();
            var queue = QueueFactory.CreateQueue(workerscount, executer, queueName);

            for (int i = 1; i <= itterationCount; i++)
            {
               // new TaskFactory().StartNew(() =>
               // {
                    queue.AddTaskAsync(QueueTask.Create(0, i.ToString())).Wait(); ;
               // }).Wait();
                var a = rnd.Next(1, 31);
                if (a % 2 == 0)
                {
                    WaitTast(2);
                }
            }
          //  WaitTast(1500);
            Assert.AreEqual(itterationCount.ToString(), executer.ExecutionTaskCount.ToString());
        }
    }
}
