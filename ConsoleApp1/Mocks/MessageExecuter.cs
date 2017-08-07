﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractQueue;
using System.Threading;

namespace ConsoleApp1.Mocks
{
    class MessageExecuter : AbstractTaskExecuter
    {
        public override void Execute(QueueTask queueTask)
        {
            switch (queueTask.Type)
            {
                case ((int)MessageTypes.A):
                    Thread.Sleep(3000);
                    MessageState.MessageA = MessageState.MessageA_Ideal;
                    this.TaskStore.SetSuccess(queueTask);
                   break;
                case ((int)MessageTypes.B):
                    Thread.Sleep(3000);
                    MessageState.MessageA = MessageState.MessageB_Ideal;
                    this.TaskStore.SetSuccess(queueTask);
                    break;
                case ((int)MessageTypes.C):
                    Thread.Sleep(3000);
                    MessageState.MessageC =  MessageState.MessageC_Ideal;
                    this.TaskStore.SetSuccess(queueTask);
                    break;
                case ((int)MessageTypes.D):
                    if (queueTask.Attempt <2)
                    throw new Exception();
                    else
                    {
                        Thread.Sleep(3000);
                        MessageState.MessageD = MessageState.MessageD_Ideal;
                    }
                    break;
            }
        }
    }

   static class  MessageState
   {
       public static string MessageA { get; set; } = "Message A not handled";
        public static string MessageB { get; set; } = "Message B not handled";
        public static string MessageC { get; set; } = "Message C not handled";
        public static string MessageD { get; set; } = "Message D not handled";


        public static string MessageA_Ideal { get; set; } = "MessageA_Ideal";
        public static string MessageB_Ideal { get; set; } = "MessageB_Ideal";
        public static string MessageC_Ideal { get; set; } = "MessageC_Ideal";
        public static string MessageD_Ideal { get; set; } = "MessageD_Ideal";
    }

    public enum MessageTypes
    {
        A,B,C,D
    }
}