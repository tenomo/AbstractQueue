using System;
using System.Linq;
using System.Threading.Tasks;
using AbstractQueue.QueueData.Entities;

namespace AbstractQueue.Core
{ 

  internal   class Queue : IQueue
    {
        /// <summary>
        /// Count workers.
        /// </summary>
        private  int queueWorkersCount;

        /// <summary>
        /// Executeble queueTask array.
        /// </summary>
        private readonly QueueWorker[] QueueWorkers;
        /// <summary>
        /// Concrete executer
        /// </summary>
        private readonly AbstractTaskExecuter Executer;
 
        private  int attemptMaxCount;

        private readonly TaskStore.TaskStore QueueTaskStore;

        public event Action<QueueTask> InProccesTaskEvent;
        public event Action<QueueTask> SuccessExecuteTaskEvent;
        public event Action<QueueTask> FailedExecuteTaskEvent;

        /// <summary>
        /// Current queue name
        /// </summary>
        public string QueueName { get; set; }

        public int AttemptMaxCount
        {
            get { return AttemptMaxCount; }
            private set
            { 
                  if (value<0)
                {
                    throw new ArgumentException("AttemptMaxCount must be more 0");
                }

                AttemptMaxCount = value;
            }
        }

        /// <summary>
        /// Count workers.
        /// </summary>
        public int QueueWorkersCount
        {
            get { return queueWorkersCount; }
            private set
            {
                if (queueWorkersCount < 0)
                    throw new ArgumentException("The queue Workers count must be more 0");
                queueWorkersCount = value;
            }
        }


        internal Queue(int queueWorkersCount, AbstractTaskExecuter executer , string queueName)
        {
            QueueName = queueName; 
            this.QueueWorkersCount = queueWorkersCount;
            Executer = executer;
            attemptMaxCount = 0;
            QueueTaskStore = new TaskStore.TaskStore();
            QueueWorkers = BuildWorkers(QueueWorkersCount, executer,queueName);
        }

        internal Queue(int queueWorkersCount, AbstractTaskExecuter executer, string queueName, int attemptMaxCount)
           : this(queueWorkersCount, executer, queueName)
        {
            AttemptMaxCount = attemptMaxCount;
            QueueWorkers = BuildWorkers(QueueWorkersCount, executer, queueName, attemptMaxCount);
        }

        private   QueueWorker[] BuildWorkers(int queueWorkersCount, AbstractTaskExecuter executer, string queueName, int attemptMaxCount = 0)
        {
            var workers = new QueueWorker[queueWorkersCount];

            for (int i = queueWorkersCount - 1; i >= 0; i--)
            {
                workers[i] = new QueueWorker(executer,queueName, attemptMaxCount);
                var buff = workers[i];

                buff.SuccessExecuteTaskEvent += OnSuccessExecuteTaskEvent;
                buff.InProccesTaskEvent += OnInProccesTaskEvent;
                buff.FailedExecuteTaskEvent += OnFailedExecuteTaskEvent;
            }
            return workers;
        }


       

        /// <summary>
        /// Add new task to queue
        /// </summary>
        /// <param name="queueTask"></param>
        /// <returns></returns>
        public int AddTask(QueueTask queueTask)
        {
            queueTask.QueueName = QueueName;
            QueueTaskStore.Add(queueTask);
            return QueueTaskStore.IndexOf(queueTask);
        }


        private void TryExecuteTask()
        {
            var worker = GetWorker();
            worker?.TryStartTask();
        }

        private QueueWorker GetWorker()
        {
            var worker = QueueWorkers.FirstOrDefault(each => each.InProccess == false);
            return worker;
        }

       

        protected virtual void OnInProccesTaskEvent(QueueTask obj)
        {
            InProccesTaskEvent?.Invoke(obj);
        }

        protected virtual void OnSuccessExecuteTaskEvent(QueueTask obj)
        {
            SuccessExecuteTaskEvent?.Invoke(obj);
        }

        protected virtual void OnFailedExecuteTaskEvent(QueueTask obj)
        {
            FailedExecuteTaskEvent?.Invoke(obj);
        }
    }
}
