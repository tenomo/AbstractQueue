

using System;
using System.Text; 
using AbstractQueue;
using Task = AbstractQueue.Task;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Queue mySyppQueue = new Queue(3, new MySuperExecuter());

            for (int i = 0; i < 20; i++)
            {
                mySyppQueue.AddTask(new Task((int)SuperTaskTypes.MySuperTaskWrapper, Encoding.UTF8.GetBytes("Helo world queue. Handle task [#] " + i.ToString())));
            }

            Console.Read();
          
        } 
        class MySuperExecuter : AbstractTaskExecuter
        {
          public  override void Execute(Task task)
            {
                switch (task.TaskType)
                {
                    case ((int)SuperTaskTypes.MySuperTaskWrapper):
                        try
                        {
                            Console.WriteLine(Encoding.UTF8.GetString(task.TaskBody)); 
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(Encoding.UTF8.GetString(task.TaskBody));
                            base.SetTaskStatus(task, ETaskStatus.Failed);
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
