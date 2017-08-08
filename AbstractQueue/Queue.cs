using System;
using System.Linq;
using System.Threading.Tasks;
using AbstractQueue.QueueData;

namespace AbstractQueue
{ 

  internal   class Queue : IExecutedTask, IQueueName, IQueue
    {
        /// <summary>
        /// Count workers.
        /// </summary>
        private readonly int QueueWorkerCount;
        /// <summary>
        /// Executeble queueTask array.
        /// </summary>
        private readonly QueueTask[] QueueWorkers;
        /// <summary>
        /// Concrete executer
        /// </summary>
        private readonly AbstractTaskExecuter Executer;

        private  bool _isHandleFailed = false;
        private  int _countHandleFailed;
        
        /// <summary>
        /// QueueTask TaskStore.
        /// </summary>
        private volatile   TaskStore.TaskStore TaskStore;

        public int CountHandleFailed
        {
            get { return _countHandleFailed; }
            private set
            {
                if (value > 1)
                    _isHandleFailed = true;
                else if (value<0)
                {
                    throw new ArgumentException("CountHandleFailed must be more 0");
                }

                _countHandleFailed = value;
            }
        }

        internal Queue(int queueWorkerCount, AbstractTaskExecuter executer , IQueueDBContext queueDbContext, string queueName)
        {
            QueueName = queueName; 
            QueueWorkerCount = queueWorkerCount;
            QueueWorkers = new QueueTask[queueWorkerCount];
            Executer = executer;
            TaskStore = new TaskStore.TaskStore(queueDbContext);
            TaskStore.ExecutedTaskEvent += TaskEventExecuted;
            TaskStore.FailedExecuteTaskEvent += TaskEventExecuted;
            _isHandleFailed = false;
            _countHandleFailed = -1;

            executer.TaskStore = TaskStore;
        }

       

        internal Queue(int queueWorkerCount, AbstractTaskExecuter executer , IQueueDBContext queueDbContext, string queueName, int countHandleFailed) : this(queueWorkerCount, executer  ,queueDbContext, queueName)
        {
            CountHandleFailed = countHandleFailed;
        }
        /// <summary>
        /// Executed queueTask handler.
        /// </summary>
        /// <param name="queueTask"></param>
        private void TaskEventExecuted(QueueTask queueTask)
        { 
                ExecutedTaskEvent?.Invoke(queueTask);
            TryStartTask(queueTask.TaskIndexInQueue);
        }

       
        public int AddTask(QueueTask queueTask)
        {
            queueTask.QueueName = QueueName;
            TaskStore.Add(queueTask);
        

            TryStartTask();

            return TaskStore.IndexOf(queueTask);
        }
        private void TryStartTask()
        {
            bool isCan = false;
            int taskId = 0;
            IsCanExecuteTask(out isCan, out taskId);
            if (isCan)
            {
                  var  executeTask =   QueueWorkers[taskId];
                executeTask.QueueTaskStatus = executeTask.QueueTaskStatus == QueueTaskStatus.Created ? QueueTaskStatus.InProcces : QueueTaskStatus.TryFailInProcces;
                 UpAttempt(executeTask);
                TaskStore.SaveChanges();
                new TaskFactory().StartNew(() =>
                {
                    try
                    {
                        Executer.Execute(executeTask);
                        TaskStore.SetSuccess(executeTask);
                    }
                    catch
                    {
                        TaskStore.SetFailed(executeTask);
                    }
                });
            }
        }

        private void TryStartTask(int index)
        {
            bool isCan = false;
            IsCanExecuteTask(out isCan, index);
            if (isCan)
            {
                var executeTask = QueueWorkers[index];
                UpAttempt(executeTask);
                executeTask.QueueTaskStatus = executeTask.QueueTaskStatus == QueueTaskStatus.Created
                    ? QueueTaskStatus.InProcces
                    : QueueTaskStatus.TryFailInProcces;
                TaskStore.SaveChanges();
                new TaskFactory().StartNew(() =>
                {
                    try
                    {
                        Executer.Execute(executeTask);
                        TaskStore.SetSuccess(executeTask);
                    }
                    catch
                    {
                        TaskStore.SetFailed(executeTask);
                    }
                });
            }
        }



        /// <summary>
        /// Check executeble queueTask and return boolean value and queueTask index.
        /// </summary>
        /// <param name="isCan"></param>
        /// <param name="index"></param>
        private void IsCanExecuteTask(out bool isCan, out int index)
        {
             index = 0;
            isCan = false;
            var queueWorkers = QueueWorkers;
            var countExecuteTasks = queueWorkers.Count(CheckQueueTaskStatus);
            var queueWorkerCount = QueueWorkerCount;
            //var task =
            //    TaskStore.GetAll().FirstOrDefault(each => CheckQueueTaskStatus(each) && each.QueueName == QueueName);
             var task = TaskStore.GetAll().FirstOrDefault(each =>  CheckQueueTaskStatus(each ) && each.QueueName == QueueName);
            isCan = countExecuteTasks < queueWorkerCount && task != null;
            if (isCan)
            {

             
                for (int i= 0; i < queueWorkerCount; i++)
                {
                    index = i;
                    var currentWorker = queueWorkers[index];
                    if (currentWorker == null || CheckQueueTaskStatus(currentWorker) ||
                        CheckTaskOnAttemptLimit(currentWorker))
                    {
                        queueWorkers[index] = task;
                        queueWorkers[index].TaskIndexInQueue = index;
                        TaskStore.SaveChanges();
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Check executeble queueTask by on executed task place and return boolean.
        /// </summary>
        /// <param name="isCan"></param>
        /// <param name="index"></param>
        /// <param name="index"></param>
        private void IsCanExecuteTask(out bool isCan,  int index)
        {
            var queueWorkers = QueueWorkers;
            var queueWorkerCount = QueueWorkerCount;
            isCan = false;
            int countExecuteTasks = queueWorkers.Count(CheckQueueTaskStatus);
            var task = TaskStore.GetAll().FirstOrDefault(each => CheckQueueTaskStatus(each) && each.QueueName == QueueName);

            isCan = countExecuteTasks < queueWorkerCount && task != null;
            if (isCan)
            {
                queueWorkers[index] = task;
                queueWorkers[index].TaskIndexInQueue = index;
                TaskStore.SaveChanges();
            }
        }
        private bool CheckQueueTaskStatus(QueueTask task  )
        {
            var _isHandleFailed = this._isHandleFailed;
            if (_isHandleFailed)
            {

                return task != null &&
                       (task?.QueueTaskStatus == QueueTaskStatus.Created || task.QueueTaskStatus == QueueTaskStatus.Failed);
        }
            else
                return task?.QueueTaskStatus == QueueTaskStatus.Created;
        }

        private bool CheckTaskOnAttemptLimit(QueueTask task)
        {
            var countHandleFailed = CountHandleFailed;
            if (task == null || _isHandleFailed == false)
                return true;
            return task.QueueTaskStatus == QueueTaskStatus.Failed && task.Attempt <= countHandleFailed;
        }

        private void UpAttempt(QueueTask task)
        {

            task.Attempt++;
            System.Diagnostics.Debug.WriteLine("UpAttempt for task id" + task.Id + ", Attempt #" + task.Attempt);
            TaskStore.SaveChanges();
        }



        public event Action<QueueTask> ExecutedTaskEvent;
        public string QueueName { get; set; }

      
    }
}
