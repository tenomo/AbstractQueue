

using System;
using System.Text;
using System.Threading.Tasks;
using AbstractQueue;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
       //     Queue mySyppQueue =   Queue.CreateQueue(3, new MySuperExecuter());
       Console.WriteLine((int)SuperTaskTypes.all);

            for (int i = 0; i < 20; i++)
            {
                string id = i.ToString();
               // new TaskFactory().StartNew(() => { mySyppQueue.AddTask(new QueueTask((int)SuperTaskTypes.MySuperTaskWrapper, Encoding.UTF8.GetBytes("Helo world queue. Handle queueTask [#] " + id.ToString()))); });
               
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
                         //   Console.WriteLine(Encoding.UTF8.GetString(queueTask.Body)); 
                            this.TaskStore.SetSuccess(queueTask);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            this.TaskStore.SetFailed(queueTask);
                        }
                    
                        break;
                }
            }
        }

        enum SuperTaskTypes
        {
            MySuperTaskWrapper = 42,
            test = 32,
            all = test | MySuperTaskWrapper
        }
         
    }
     
}
