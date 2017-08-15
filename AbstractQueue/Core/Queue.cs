using System;
using System.Linq;
using System.Threading.Tasks;
using AbstractQueue.QueueData.Entities;

namespace AbstractQueue.Core
{

    internal class Queue : IQueue
    {
        /// <summary>
        /// Count workers.
        /// </summary>
        private int queueWorkersCount;

        /// <summary>
        /// Executeble queueTask array.
        /// </summary>
        private readonly QueueWorker[] QueueWorkers;


        /// <summary>
        /// Concrete executer
        /// </summary>
        private BehaviorTaskExecution Executer
        {
            get { return _executer; }
            set
            {
                if (_executer == null)
                    throw new NullReferenceException("Executer must be not null");
                _executer = value;
            }
        }

        private int attemptMaxCount;

        private TaskStore.TaskStore QueueTaskStore { get; set; }
        private BehaviorTaskExecution _executer;
        private string _queueName;

      

        /// <summary>
        /// Current queue name
        /// </summary>
        public string QueueName
        {
            get { return _queueName; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("QueueName must be not null and not be empty");
                _queueName = value;
            }
        }

        public int AttemptMaxCount
        {
            get { return attemptMaxCount; }
            private set
            {
                if (value < 0)
                {
                    throw new ArgumentException("AttemptMaxCount must be more 0");
                }

                attemptMaxCount = value;
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


        internal Queue(int queueWorkersCount, BehaviorTaskExecution executer, string queueName)
        {
            QueueName = queueName;
            this.QueueWorkersCount = queueWorkersCount;
            _executer = executer;
            attemptMaxCount = 0;
            QueueTaskStore = new TaskStore.TaskStore(QueueName);
            QueueWorkers = BuildWorkers(QueueWorkersCount, Executer, QueueName);
           
        }

        internal Queue(int queueWorkersCount, BehaviorTaskExecution executer, string queueName, int attemptMaxCount)
            : this(queueWorkersCount, executer, queueName)
        {
            
            AttemptMaxCount = attemptMaxCount;
            QueueWorkers = BuildWorkers(QueueWorkersCount, Executer, QueueName, attemptMaxCount);
        }

        private QueueWorker[] BuildWorkers(int queueWorkersCount, BehaviorTaskExecution executer, string queueName,
            int attemptMaxCount = 0)
        {
            var workers = new QueueWorker[queueWorkersCount];

            for (int i = queueWorkersCount - 1; i >= 0; i--)
            {
                workers[i] = new QueueWorker(executer, queueName, attemptMaxCount);
            }
            return workers;
        }

        static object obj = new object();

        /// <summary>
        /// Add new task to queue
        /// </summary>
        /// <param name="queueTask"></param>
        /// <returns></returns>
        public int AddTask(QueueTask queueTask)
        {
            lock (obj)
            {
                queueTask.QueueName = QueueName;
                QueueTaskStore.Add(queueTask);
                TryExecuteTask();
                return QueueTaskStore.IndexOf(queueTask);
            }
        }

        public async Task<int> AddTaskAsync(QueueTask queueTask)
        {
            return await new TaskFactory().StartNew( ()=> AddTask(queueTask));
        }

        private void TryExecuteTask()
        {
            var worker = GetWorker();
            worker?.TryStartTask();
        }

        private QueueWorker GetWorker()
        {
           
            var worker = QueueWorkers.FirstOrDefault(each => !each.InProccess);
            
            return worker;

        }

      
    }
}
