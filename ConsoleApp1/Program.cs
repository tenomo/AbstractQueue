﻿

using System;
using System.Text; 
using AbstractQueue;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Queue mySyppQueue = new Queue(3, new MySuperExecuter());

            for (int i = 0; i < 20; i++)
            {
                mySyppQueue.AddTask(new QueueTask((int)SuperTaskTypes.MySuperTaskWrapper, Encoding.UTF8.GetBytes("Helo world queue. Handle queueTask [#] " + i.ToString())));
            }

            Console.Read();
          
        } 
        class MySuperExecuter : AbstractTaskExecuter
        {
          public  override void Execute(QueueTask queueTask)
            {
                switch (queueTask.Type)
                {
                    case ((int)SuperTaskTypes.MySuperTaskWrapper):
                        try
                        {
                            Console.WriteLine(Encoding.UTF8.GetString(queueTask.Body)); 
                            queueTask.SetSuccess();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            queueTask.SetFailed();
                        }
                    
                        break;
                }
            }
             
        }

        enum SuperTaskTypes
        {
            MySuperTaskWrapper = 42
        }
         
    }
     
}
