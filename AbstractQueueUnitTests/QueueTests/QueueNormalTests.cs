﻿using System;
using System.Threading;
using System.Threading.Tasks;
using AbstractQueue.Core;
using AbstractQueue.QueueData.Entities;
using AbstractQueueUnitTests.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AbstractQueueUnitTests.QueueTests
{
    [TestClass]
    public class QueueNormalTests
    {
       

       

        public QueueNormalTests()
        {

        }

        private void WaitTast(int timeout)
        {
            Thread.Sleep(timeout);
        }


        #region 10_Iterations
        [TestMethod]
        public void execute_tasks_1_worker_10_Iterations()
        {
            int itterationCount = 10;
            const string queueName = "execute_tasks_1_worker_10_Iterations";
            int workerscount = 1;
            var executer = new MockBehaviorTaskExecution();
            var queue = QueueFactory.CreateQueue(workerscount, executer, queueName);

            for (int i = 1; i <= itterationCount; i++)
            {
                queue.AddTask(QueueTask.Create(0, i.ToString()));
            }
            WaitTast(100);
            Assert.AreEqual(itterationCount.ToString(), executer.ExecutionTaskCount.ToString());
        }

        [TestMethod]
        public void execute_tasks_4_worker_10_Iterations()
        {
            int itterationCount = 10;
            const string queueName = "execute_tasks_4_worker_10_Iterations";
            int workerscount = 4;

            var executer = new MockBehaviorTaskExecution();

            var queue = QueueFactory.CreateQueue(workerscount, executer, queueName);

            for (int i = 1; i <= itterationCount; i++)
            {
                queue.AddTask(QueueTask.Create(0, i.ToString()));
            }
            WaitTast(100);
            Assert.AreEqual(itterationCount.ToString(), executer.ExecutionTaskCount.ToString());
        }



        #endregion

        #region 100_Iterations
        [TestMethod]
        public void execute_tasks_1_worker_100_Iterations()
        {
            int itterationCount = 100;
            const string queueName = "execute_tasks_1_worker_100_Iterations";
            int workerscount = 1;
            var executer = new MockBehaviorTaskExecution();
            var queue = QueueFactory.CreateQueue(workerscount, executer, queueName);

            for (int i = 1; i <= itterationCount; i++)
            {
                queue.AddTask(QueueTask.Create(0, i.ToString()));
            }
            WaitTast(100);
            Assert.AreEqual(itterationCount.ToString(), executer.ExecutionTaskCount.ToString());
        }

        [TestMethod]
        public void execute_tasks_4_worker_100_Iterations()
        {
            int itterationCount = 100;
            const string queueName = "execute_tasks_4_worker_100_Iterations";
            int workerscount = 4;

            var executer = new MockBehaviorTaskExecution();

            var queue = QueueFactory.CreateQueue(workerscount, executer, queueName);

            for (int i = 1; i <= itterationCount; i++)
            {
                queue.AddTask(QueueTask.Create(0, i.ToString()));
            }
            WaitTast(100);
            Assert.AreEqual(itterationCount.ToString(), executer.ExecutionTaskCount.ToString());
        }


       

        

        #endregion

   

    }
}
