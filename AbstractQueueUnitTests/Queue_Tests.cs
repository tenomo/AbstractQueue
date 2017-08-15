using System;
using System.Threading;
using System.Threading.Tasks;
using AbstractQueue.Core;
using AbstractQueue.QueueData.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AbstractQueueUnitTests
{
    [TestClass]
    public class Queue_Tests
    {
        class MockTaskExecution : BehaviorTaskExecution
        {
            private int _executionTaskCount = 0;
            private object lockObj = new object();

            public int ExecutionTaskCount
            {
                get
                {
                    lock (lockObj)
                    {
                        return _executionTaskCount;
                    }
                }
                set
                {
                    lock (lockObj)
                    {
                        _executionTaskCount = value;

                    }

                }
            }

            public override void Execute(QueueTask queueTask)
            {
                ExecutionTaskCount++;
            }
        }

        class MockErorrTaskExecution : MockTaskExecution
        {
            public override void Execute(QueueTask queueTask)
            {
                if (queueTask.Attempt < 2)
                    throw new ExecutionEngineException();
                base.Execute(queueTask);
            }
        }

        public Queue_Tests()
        {

        }

        private void WaitTast(int timeout)
        {
            WaitTast(timeout);
        }

    [TestMethod]
        public void execute_tasks_1_worker_10_Iterations()
        {
            int itterationCount = 10;  
            const string queueName = "execute_tasks_1_worker_10_Iterations";
            int workerscount = 1;
            var executer = new MockTaskExecution();
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

            var executer = new MockTaskExecution();

            var queue = QueueFactory.CreateQueue(workerscount, executer, queueName);

            for (int i = 1; i <= itterationCount; i++)
            {
                queue.AddTask(QueueTask.Create(0, i.ToString()));
            }
            WaitTast(100);
            Assert.AreEqual(itterationCount.ToString(), executer.ExecutionTaskCount.ToString());
        }


        Random rnd = new Random();
        [TestMethod]
        public void execute_task_4_worker_self_tread_test()
        {
            int itterationCount = 10;
            const string queueName = "execute_task_4_worker_test";
            int workerscount = 4;
            var executer = new MockTaskExecution();
            var queue = QueueFactory.CreateQueue(workerscount, executer, queueName);

            for (int i = 1; i <= itterationCount; i++)
            {
                new TaskFactory().StartNew(() =>
                {
                    queue.AddTask(QueueTask.Create(0, i.ToString()));
                });
                var a = rnd.Next(1, 31);
                if (a  % 2 == 0)
                {
                    WaitTast(100);
                }
            }
            WaitTast(10000);
            Assert.AreEqual(itterationCount.ToString(), executer.ExecutionTaskCount.ToString());
        }



        //[TestMethod]
        //public void ExecuteTasks_on_2_Workers_handle_error_Test()
        //{
        //    int itterationCount = 10;
        //    int executedTasksCount = 1;
        //    const string queueName = "ExecuteTasks_on_2_Workers_handle_error_Test";
        //    int workerscount = 4;
        //    int attemptsCount = 3;
        //    var queue = QueueFactory.CreateQueueHandleFailed(workerscount, new MockTaskExecution(), attemptsCount, queueName);
        //    //queue.TaskExecutionEvents.SuccessExecuteTaskEvent += delegate (ITaskStore store, QueueTask task)
        //    //{
        //    //    executedTasksCount++;
        //    //};


        //    for (int i = 0; i < itterationCount; i++)
        //    {
        //        queue.AddTask(QueueTask.Create(0, i.ToString()));
        //    }
        //    Assert.AreEqual(itterationCount, executedTasksCount);

        //}
    }
}
