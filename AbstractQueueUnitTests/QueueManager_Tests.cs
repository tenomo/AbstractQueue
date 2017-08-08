using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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

        /// <summary>
        /// Test queue with QueueManager on NumberCalculateExecuter with two workers
        /// </summary>
        [TestMethod]
        public void Test_Queue_QM_On_NumberCalculateExecuter_with_2_Workers()
        {
           const string  queueName = "QM_On_NumberCalculateExecuter_2_Workers";
            var queue = QueueFactory.CreateQueue(2, new NumberCalculateExecuter(), QueueDbContext, queueName);
            QueueManager.Kernel.RegistrateQueue(queue);
            
            var calculationTypeArray  = Enum.GetValues(typeof(CaluculationType)).Cast<CaluculationType>().ToArray();
            object body;
            ExecutionResult[] resultList = new ExecutionResult[calculationTypeArray.Length];
            // build ideal
            for (int i = 0; i < calculationTypeArray.Length; i++)
            {
                if (calculationTypeArray[i] != CaluculationType.Sqrt ||
                    calculationTypeArray[i] != CaluculationType.Factorial)
                {
                    body = new NumberWraper2D  { X =  i * 3,Y= i * 2};
                }
                else
                {
                    body = new NumberWraper1D { X = i * 3 };
                }
                var jsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(body);
                NumberCalculateExecuter.Calculator(QueueTask.Create((byte) calculationTypeArray[i], jsonBody));
                 
            }
            NumberCalculateExecuter.resultList.CopyTo(resultList);
            NumberCalculateExecuter.resultList= new List<ExecutionResult>();
            Assert.AreEqual(0 , NumberCalculateExecuter.resultList.Count);


              for (int i = 0; i < calculationTypeArray.Length; i++)
            {
                if (calculationTypeArray[i] != CaluculationType.Sqrt ||
                    calculationTypeArray[i] != CaluculationType.Factorial)
                {
                    body = new NumberWraper2D  { X =  i * 3,Y= i * 2};
                }
                else
                {
                    body = new NumberWraper1D { X = i * 3 };
                }
                var jsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(body);
                QueueManager.Kernel[queueName].AddTask(QueueTask.Create((byte) calculationTypeArray[i], jsonBody));

            }


        }
    }
}
 
