﻿using System;
using System.Threading;
using AbstractQueue;
using AbstractQueueUnitTests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AbstractQueueUnitTests
{
    [TestClass]
    public class Queue_Tests
    {
        private QueueDBContext QueueDbContext;
        public Queue_Tests()
        {
            QueueDbContext = new QueueDBContext();
        }

       
        [TestMethod]
        public void ExecuteTasks_on_1_Worker_Test()
        {
          
            var queue =  QueueFactory.CreateQueue(1, new MessageExecuter(), QueueDbContext, "ExecuteTasks_on_1_Worker");
            queue.AddTask(QueueTask.Create((int)MessageTypes.A, " "));
            queue.AddTask(QueueTask.Create((int)MessageTypes.B, " "));
            queue.AddTask(QueueTask.Create((int)MessageTypes.C, " "));
            queue.AddTask(QueueTask.Create((int)MessageTypes.D, " "));
            Thread.Sleep(5000);
                 Assert.AreEqual(MessageState.MessageA_Ideal, MessageState.MessageA);
                    Assert.AreEqual(MessageState.MessageB_Ideal, MessageState.MessageB);
                    Assert.AreEqual(MessageState.MessageC_Ideal, MessageState.MessageC);
                    Assert.AreNotEqual(MessageState.MessageD_Ideal, MessageState.MessageD);
           
        }

         
        [TestMethod]
        public void ExecuteTasks_on_4_Workers_Test()
        {
            
            var queue = QueueFactory.CreateQueue(4, new MessageExecuter(), QueueDbContext, "ExecuteTasks_on_4_Worker");
          queue.AddTask(QueueTask.Create((int)MessageTypes.A, " "));
            queue.AddTask(QueueTask.Create((int)MessageTypes.B, " "));
            queue.AddTask(QueueTask.Create((int)MessageTypes.C, " "));
            queue.AddTask(QueueTask.Create((int)MessageTypes.D, " "));
            Thread.Sleep(3500);
            Assert.AreEqual(MessageState.MessageA_Ideal, MessageState.MessageA);
            Assert.AreEqual(MessageState.MessageB_Ideal, MessageState.MessageB);
            Assert.AreEqual(MessageState.MessageC_Ideal, MessageState.MessageC);
            Assert.AreNotEqual(MessageState.MessageD_Ideal, MessageState.MessageD);

        }



        [TestMethod]
        public void ExecuteTasks_on_2_Workers_handle_error_Test()
        {

            var queue = QueueFactory.CreateQueueHandleFailed(4,new MessageExecuter(), 4,QueueDbContext, "ExecuteTasks_on_2_Workers_handle_error_Test");
            queue.AddTask(QueueTask.Create((int)MessageTypes.A, "A"));
            queue.AddTask(QueueTask.Create((int)MessageTypes.B, "B"));
            queue.AddTask(QueueTask.Create((int)MessageTypes.C, "C"));
            queue.AddTask(QueueTask.Create((int)MessageTypes.D, "D"));
            Thread.Sleep(5000);
            Assert.AreEqual(MessageState.MessageA_Ideal, MessageState.MessageA);
            Assert.AreEqual(MessageState.MessageB_Ideal, MessageState.MessageB);
            Assert.AreEqual(MessageState.MessageC_Ideal, MessageState.MessageC);
            Assert.AreEqual(MessageState.MessageD_Ideal, MessageState.MessageD);

        }
    }
}
