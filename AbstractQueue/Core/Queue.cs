using System;
using System.Linq;
using AbstractQueue.Infrastructure;
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
        private AbstractTaskExecuter Executer
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

        private readonly TaskStore.TaskStore QueueTaskStore;
        private AbstractTaskExecuter _executer;
        private string _queueName;

        //public event Action<QueueTask> InProccesTaskEvent;
        //public event Action<QueueTask> SuccessExecuteTaskEvent;
        //public event Action<QueueTask> FailedExecuteTaskEvent;

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
            get { return AttemptMaxCount; }
            private set
            {
                if (value < 0)
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


        internal Queue(int queueWorkersCount, AbstractTaskExecuter executer, string queueName)
        {
            QueueName = queueName;
            this.QueueWorkersCount = queueWorkersCount;
            _executer = executer;
            attemptMaxCount = 0;
            QueueTaskStore = new TaskStore.TaskStore(QueueName);
            QueueWorkers = BuildWorkers(QueueWorkersCount, Executer, QueueName);
        }

        internal Queue(int queueWorkersCount, AbstractTaskExecuter executer, string queueName, int attemptMaxCount)
            : this(queueWorkersCount, executer, queueName)
        {
            
            AttemptMaxCount = attemptMaxCount;
            QueueWorkers = BuildWorkers(QueueWorkersCount, Executer, QueueName, attemptMaxCount);
        }

        private QueueWorker[] BuildWorkers(int queueWorkersCount, AbstractTaskExecuter executer, string queueName,
            int attemptMaxCount = 0)
        {
            var workers = new QueueWorker[queueWorkersCount];

            for (int i = queueWorkersCount - 1; i >= 0; i--)
            {
                workers[i] = new QueueWorker(executer, queueName, attemptMaxCount);
                var buff = workers[i];

                //buff.SuccessExecuteTaskEvent += OnSuccessExecuteTaskEvent;
                //buff.InProccesTaskEvent += OnInProccesTaskEvent;
                //buff.FailedExecuteTaskEvent += OnFailedExecuteTaskEvent;
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
            TryExecuteTask();
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

        public ITaskExecutionObserver TaskExecutionEvents => Infrastructure.TaskExecutionObserver.Kernal;


        //private void OnInProccesTaskEvent(QueueTask obj)
        //{
        //    InProccesTaskEvent?.Invoke(obj);
        //}

        //private void OnSuccessExecuteTaskEvent(QueueTask obj)
        //{
        //    SuccessExecuteTaskEvent?.Invoke(obj);
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="obj"></param>
        //private void OnFailedExecuteTaskEvent(QueueTask obj)
        //{
        //    FailedExecuteTaskEvent?.Invoke(obj);
        //}
    }
}
