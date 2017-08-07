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
        private readonly int ThreadCount;
        /// <summary>
        /// Executeble queueTask array.
        /// </summary>
        private readonly QueueTask[] QueueTasks;
        /// <summary>
        /// Concrete executer
        /// </summary>
        private readonly AbstractTaskExecuter Executer;

        private  bool _isHandleFailed = false;
        private  int _countHandleFailed;
        
        /// <summary>
        /// QueueTask TaskStore.
        /// </summary>
        private readonly  TaskStore.TaskStore TaskStore;

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

        internal Queue(int threadCount, AbstractTaskExecuter executer , IQueueDBContext queueDbContext, string queueName)
        {
            QueueName = queueName; 
            ThreadCount = threadCount;
            QueueTasks = new QueueTask[threadCount];
            Executer = executer;
            TaskStore = new TaskStore.TaskStore(queueDbContext);

            _isHandleFailed = false;
            _countHandleFailed = -1;

            executer.TaskStore = TaskStore;
        }
        internal Queue(int threadCount, AbstractTaskExecuter executer , IQueueDBContext queueDbContext, string queueName, int countHandleFailed) : this(threadCount, executer  ,queueDbContext, queueName)
        {
            CountHandleFailed = countHandleFailed;
        }
        /// <summary>
        /// Executed queueTask handler.
        /// </summary>
        /// <param name="queueTask"></param>
        private void Task_ExecutedTask(QueueTask queueTask)
        {
            if (queueTask.QueueTaskStatus == QueueTaskStatus.Success)
            {
               
                queueTask.QueueTaskStatus = QueueTaskStatus.Success;
                queueTask.ExecutedDate = DateTime.Now;
                TaskStore.SaveChanges();
                ExecutedTask?.Invoke(queueTask);
            }
            TryStartTask(queueTask.TaskIdInQueue);
        }

        private void TryStartTask()
        {
            bool isCan = false;
            int taskId = 0;
            IsCanExecuteTask(out isCan, out taskId);
            if (isCan )
            {
                var executeTask = QueueTasks[taskId];
               
                executeTask.QueueTaskStatus = executeTask.QueueTaskStatus == QueueTaskStatus.Created ? QueueTaskStatus.InProcces: QueueTaskStatus.TryFailInProcces;
                TaskStore.SaveChanges();
                new TaskFactory().StartNew(() =>
                {
                    try
                    {
                        TaskStore.ExecutedTask += Task_ExecutedTask;
                        Executer.Execute(executeTask);
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
                var executeTask = QueueTasks[index];

                executeTask.QueueTaskStatus = executeTask.QueueTaskStatus == QueueTaskStatus.Created ? QueueTaskStatus.InProcces : QueueTaskStatus.TryFailInProcces;
                TaskStore.SaveChanges();
                new TaskFactory().StartNew(() =>
                {
                    try
                    {
                        TaskStore.ExecutedTask += Task_ExecutedTask;
                        Executer.Execute(executeTask);
                    }
                    catch
                    {
                        TaskStore.SetFailed(executeTask);
                    }
                });
            }
        }

        public int AddTask(QueueTask queueTask)
        {
            TaskStore.Add(queueTask);

            TryStartTask();
           
            return TaskStore.IndexOf(queueTask);
        }

        /// <summary>
        /// Check executeble queueTask and return boolean value and queueTask index.
        /// </summary>
        /// <param name="isCan"></param>
        /// <param name="index"></param>
        private void IsCanExecuteTask(out bool isCan, out int index)
        {
            index = -1;
            int countExecuteTasks = QueueTasks.Count(CheckQueueTaskStatus);

            var task = TaskStore.FirstOrDefault(each => CheckQueueTaskStatus(each ) && each.QueueName == QueueName);
            isCan = countExecuteTasks < ThreadCount && task != null;
            if (isCan)
                for (index = 0; index < ThreadCount; index++)
                {
                    var currentWorker = QueueTasks[index];
                    if (currentWorker == null || CheckQueueTaskStatus(currentWorker ) || CheckTaskOnAttemptLimit(currentWorker))
                    { 
                         
                        QueueTasks[index] = task;
                        QueueTasks[index].TaskIdInQueue = index;
                        return;
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
            
            int countExecuteTasks = QueueTasks.Count(CheckQueueTaskStatus);
            var task = TaskStore.FirstOrDefault(each => CheckQueueTaskStatus(each) && each.QueueName == QueueName);

            isCan = countExecuteTasks < ThreadCount && task != null;
            if (isCan)
            {
                QueueTasks[index] = task;
                QueueTasks[index].TaskIdInQueue = index;
            }
        }
        private bool CheckQueueTaskStatus(QueueTask task  )
        {
            if (_isHandleFailed)
                return task?.QueueTaskStatus == QueueTaskStatus.Created || task.QueueTaskStatus == QueueTaskStatus.Failed;
            else
                return task?.QueueTaskStatus == QueueTaskStatus.Created;
        }

        private bool CheckTaskOnAttemptLimit(QueueTask task)
        {
            if (task == null || _isHandleFailed == false)
                return true;
            if (task.QueueTaskStatus == QueueTaskStatus.Failed && task.Attempt >= CountHandleFailed)
                return false;
            else
            {
                return false;
            }
        }

        #region Factory methods

       

        public event Action<QueueTask> ExecutedTask;
        public string QueueName { get; set; }

        #endregion
    }
}
