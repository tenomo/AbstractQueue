using System.Runtime.Remoting;
using System.Threading.Tasks;
using AbstractQueue.Core;
using AbstractQueue.QueueData.Entities;
using AbstractQueue.TaskStore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

            queue_names = new[] { QueueNameOne, QueueNameTwo, QueueNameThree };
            //    context =DBSingle.DbContext;
        }
        [TestMethod]

        public void Test_Get_Queue()
        {
            // var context = new QueueDBContext();
            var queue_one = QueueFactory.CreateQueue(2, new MockTaskExecuter(), QueueNameOne);
            var queue_two = QueueFactory.CreateQueue(3, new MockTaskExecuter(), QueueNameTwo);
            var queue_three = QueueFactory.CreateQueue(4, new MockTaskExecuter(), QueueNameThree);


            QueueManager.Kernal.RegistrateQueue(queue_one);
            QueueManager.Kernal.RegistrateQueue(queue_two);
            QueueManager.Kernal.RegistrateQueue(queue_three);


            Assert.AreEqual(queue_one, QueueManager.Kernal[QueueNameOne]);
            Assert.AreEqual(queue_two, QueueManager.Kernal[QueueNameTwo]);
            Assert.AreEqual(queue_three, QueueManager.Kernal[QueueNameThree]);

        }

        [TestMethod]

        public void Test_Delete_Queue()
        {
            int queue_s_Count = QueueManager.Kernal.QueuesCount;
            System.Diagnostics.Debug.WriteLine(queue_s_Count);
            foreach (var VARIABLE in queue_names)
            {
                QueueManager.Kernal.DeleteQueue(VARIABLE);
            }


            int ideal = queue_s_Count - 3;
            System.Diagnostics.Debug.WriteLine(ideal);
            Assert.AreEqual(QueueManager.Kernal.QueuesCount, ideal);

        }

        /// <summary>
        /// Test queue with QueueManager on NumberCalculateExecuter with two workers
        /// </summary>
        [TestMethod]
        public void Test_Queue_QM_On_NumberCalculateExecuter_with_2_Workers()
        {
            int itterationCount = 10;
            object executedTasksCount = 1;
            const string queueName = "Test_Queue_QM_On_NumberCalculateExecuter_with_2_Workers";
            var queue = QueueFactory.CreateQueue(2, new MockTaskExecuter(), queueName);
        
            QueueManager.Kernal.RegistrateQueue(queue);

            for (int i = 0; i < itterationCount; i++)
            {
                QueueManager.Kernal[queueName].AddTask(QueueTask.Create(0, i.ToString()));
            }
            Assert.AreEqual(itterationCount, executedTasksCount);
        }




        class MockTaskExecuter : AbstractTaskExecuter
        {
            public override void Execute(QueueTask queueTask)
            {

            }
        }

    }
}


