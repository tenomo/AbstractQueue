using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AbstractQueue;
using AbstractQueueUnitTests.Mocks;
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
            var context = new QueueDBContext();
            var queue_one = QueueFactory.CreateQueue(2, new MessageExecuter(), context, QueueNameOne);
            var queue_two = QueueFactory.CreateQueue(3, new MessageExecuter(), context, QueueNameTwo);
            var queue_three = QueueFactory.CreateQueue(4, new MessageExecuter(), context, QueueNameThree);


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
            var context = new QueueDBContext();
           
         
         
            var calculationTypeArray  = Enum.GetValues(typeof(CaluculationType)).Cast<CaluculationType>().ToArray();
            object body;
            ExecutionResult[] resultList = new ExecutionResult[calculationTypeArray.Length];
            // build ideal
            for (int i = 0; i < calculationTypeArray.Length; i++)
            {
                int val = i + 1;
                if (calculationTypeArray[i] != CaluculationType.Sqrt ||
                    calculationTypeArray[i] != CaluculationType.Factorial)
                {
                    body = new NumberWraper2D  { X = val * 3,Y= val * 2};
                }
                else
                {
                    body = new NumberWraper1D { X = val * 3 };
                }
                var jsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(body);
                NumberCalculateExecuter.Calculator(QueueTask.Create((byte) calculationTypeArray[i], jsonBody));
                Logger.LogMessage("Ideal. Type: " + NumberCalculateExecuter.resultList[i].Type + " Result:" +NumberCalculateExecuter.resultList[i].Result);
            }
            NumberCalculateExecuter.resultList.CopyTo(resultList);
            NumberCalculateExecuter.resultList= new List<ExecutionResult>();
            Assert.AreEqual(0 , NumberCalculateExecuter.resultList.Count);

            const string queueName = "lala";
            var queue = QueueFactory.CreateQueue(6, new NumberCalculateExecuter(), context, queueName);
            QueueManager.Kernel.RegistrateQueue(queue);
            Logger.LogMessage(QueueManager.Kernel[queueName].QueueName + "\n");


            for (int i = 0; i < calculationTypeArray.Length; i++)
            {
                int val = i + 1;
                if (calculationTypeArray[i] != CaluculationType.Sqrt ||
                    calculationTypeArray[i] != CaluculationType.Factorial)
                {
                    body = new NumberWraper2D  { X = val * 3,Y= val * 2};
                }
                else
                {
                    body = new NumberWraper1D { X = val * 3 };
                } 
                var jsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(body);
                QueueManager.Kernel[queueName].AddTask(QueueTask.Create((byte) calculationTypeArray[i], jsonBody));

            }


        }
    }
}
 
