//using System;
//using AbstractQueue.Infrastructure;
//using AbstractQueue.QueueData.Entities;
//using AbstractQueue.TaskStore;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace AbstractQueueUnitTests
//{
//    [TestClass]
//    public class TaskObserverTests
//    {
//        [TestMethod]
//        public void TestMethod1()
//        {
//         Assert.IsNotNull(   TaskExecutionObserver.Kernal.ToString());
//            int a = 0;
//            TaskExecutionObserver.Kernal.FailedExecuteTaskEvent +=delegate(ITaskStore store, QueueTask task) { a++; };
//            TaskExecutionObserver.Kernal.OnFailedExecuteTaskEvent(null,null);

//            Assert.AreEqual(1,a);
//        }
//    }
//}
