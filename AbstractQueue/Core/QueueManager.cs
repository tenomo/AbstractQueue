using System.Collections.Generic;

namespace AbstractQueue.Core
{
    public sealed  class QueueManager  
    {

        /// защищённый конструктор нужен, чтобы предотвратить создание экземпляра класса Singleton
        protected QueueManager()
        {
            queues = new Dictionary<string, IQueue>();
            
        }

        private sealed class QueueManagerCreator
        {
            private static readonly QueueManager instance = new QueueManager();
            public static QueueManager Instance => instance;
        }


        public static QueueManager Kernel => QueueManagerCreator.Instance;


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


        public static Config Config { get; set; }

    }
}
 
