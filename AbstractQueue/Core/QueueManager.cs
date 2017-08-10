using System;
using System.Collections.Generic;
using System.Linq;
using AbstractQueue.Infrastructure;
using AbstractQueue.TaskStore;
using System.Timers;
using AbstractQueue.QueueData.Entities;

namespace AbstractQueue.Core
{
    public sealed  class QueueManager   : Singleton<QueueManager>
    {
        private ITaskStore taslStore;
        private Timer timer;
        /// защищённый конструктор нужен, чтобы предотвратить создание экземпляра класса Singleton
        protected QueueManager()
        {
            queues = new Dictionary<string, IQueue>();
            taslStore = new TaskStore.TaskStore();
            timer = new Timer();
            int hourAsMs = 3600000;
            timer.Interval = hourAsMs;
            timer.Elapsed += TaskCleaner;
        }

        private void TaskCleaner(object sender, ElapsedEventArgs e)
        {
          var tasks =   taslStore.Where(each=>each.QueueTaskStatus == QueueTaskStatus.Created ||  each.QueueTaskStatus == QueueTaskStatus.Failed).ToList();

            tasks.ForEach(delegate(QueueTask task)
            {
                if (task != null)
                    throw new NullReferenceException($"At clear executed tasks task is null");

               if( task.ExecutedDate.HasValue)
                    throw new NullReferenceException($"At clear executed tasks, execution date is null. Task:{task} ");

                var executionDate = task.ExecutedDate.Value;
               if ( executionDate.Subtract(System.DateTime.Now).TotalHours >= 48 )
                    taslStore.DeleteById(task.Id);
            } );
    
        }

        private Dictionary<string, IQueue> queues { get; set; }

        public void RegistrateQueue(IQueue queue)
        {
            queues.Add(queue.QueueName, queue);
        }

        public void DeleteQueue(string queueName)
        {
            queues.Remove(queueName);
        }

        public void DeleteQueue(params string[] queueNames)
        {
            foreach (var queueName in queueNames)
            {
                DeleteQueue(queueNames);
            }
        }

        public int QueuesCount => queues.Count;

        public IQueue this[string QueueName] => queues[QueueName];


        public   Config Config { get; set; }

    }
}
 
