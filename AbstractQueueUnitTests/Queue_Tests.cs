using System;
using System.Threading;
using AbstractQueue;
using AbstractQueue.Core;
using AbstractQueue.QueueData.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AbstractQueueUnitTests
{
    [TestClass]
    public class Queue_Tests
    {
        class MockTaskExecuter : AbstractTaskExecuter
        {
            public override void Execute(QueueTask queueTask)
            {

            }
        }
        public Queue_Tests()
        {

        }


        [TestMethod]
        public void ExecuteTasks_on_1_Worker_Test()
        {
            int itterationCount = 10;
            int executedTasksCount = 1;
            const string queueName = "ExecuteTasks_on_1_Worker_Test";
            int workerscount = 1;
            var queue = QueueFactory.CreateQueue(workerscount, new MockTaskExecuter(), queueName);
            queue.SuccessExecuteTaskEvent += delegate (QueueTask task)
            {
                executedTasksCount++;
            };
         

            for (int i = 0; i < itterationCount; i++)
            {
                queue.AddTask(QueueTask.Create(0, i.ToString()));
            }
            Assert.AreEqual(itterationCount, executedTasksCount);

        }


        [TestMethod]
        public void ExecuteTasks_on_4_Workers_Test()
        {
            int itterationCount = 10;
            int executedTasksCount = 1;
            const string queueName = "ExecuteTasks_on_4_Workers_Test";
            int workerscount = 4;
            var queue = QueueFactory.CreateQueue(workerscount, new MockTaskExecuter(), queueName);
            queue.SuccessExecuteTaskEvent += delegate (QueueTask task)
            {
                executedTasksCount++;
            };


            for (int i = 0; i < itterationCount; i++)
            {
                queue.AddTask(QueueTask.Create(0, i.ToString()));
            }
            Assert.AreEqual(itterationCount, executedTasksCount);


        }



        [TestMethod]
        public void ExecuteTasks_on_4_Workers_handle_error_Test()
        {
            int itterationCount = 10;
            int executedTasksCount = 1;
            const string queueName = "ExecuteTasks_on_4_Workers_handle_error_Test";
            int workerscount = 2;
            int attemptsCount = 3;
            var queue = QueueFactory.CreateQueueHandleFailed(workerscount, new MockTaskExecuter(), attemptsCount, queueName);
            queue.SuccessExecuteTaskEvent += delegate (QueueTask task)
            {
                executedTasksCount++;
            };


            for (int i = 0; i < itterationCount; i++)
            {
                queue.AddTask(QueueTask.Create(0, i.ToString()));
            }
            Assert.AreEqual(itterationCount, executedTasksCount);

        }


        [TestMethod]
        public void ExecuteTasks_on_2_Workers_handle_error_Test()
        {
            int itterationCount = 10;
            int executedTasksCount = 1;
            const string queueName = "ExecuteTasks_on_2_Workers_handle_error_Test";
            int workerscount = 4;
            int attemptsCount = 3;
            var queue = QueueFactory.CreateQueueHandleFailed(workerscount, new MockTaskExecuter(), attemptsCount, queueName);
            queue.SuccessExecuteTaskEvent += delegate (QueueTask task)
            {
                executedTasksCount++;
            };


            for (int i = 0; i < itterationCount; i++)
            {
                queue.AddTask(QueueTask.Create(0, i.ToString()));
            }
            Assert.AreEqual(itterationCount, executedTasksCount);

        }
    }
}
