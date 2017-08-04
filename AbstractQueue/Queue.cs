using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        /// <summary>
        /// QueueTask TaskStore.
        /// </summary>
        private readonly TaskStore TaskStore;


     

        internal Queue(int threadCount, AbstractTaskExecuter executer , IQueueDBContext queueDbContext)
        {
            ThreadCount = threadCount;
            QueueTasks = new QueueTask[threadCount];
            Executer = executer;
            TaskStore = new TaskStore(queueDbContext);

            _isHandleFailed = false;
            _countHandleFailed = -1;

            executer.TaskStore = TaskStore;
        }

       

        internal Queue(int threadCount, AbstractTaskExecuter executer , IQueueDBContext queueDbContext, int countHandleFailed) : this(threadCount, executer  ,queueDbContext)
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
            TryStartTask();
            
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
                        return;
                    }
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
