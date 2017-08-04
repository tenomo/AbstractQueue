using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractQueue
{
  public sealed class Queue : IExecutedTask
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

        private readonly bool _isHandleFailed;
        private readonly int _countHandleFailed;

        /// <summary>
        /// QueueTask _taskStore.
        /// </summary>
        private readonly TaskStore _taskStore;


       

        private Queue(int threadCount, AbstractTaskExecuter executer)
        {
            ThreadCount = threadCount;
            QueueTasks = new QueueTask[threadCount];
            Executer = executer;
            _taskStore = new TaskStore();
            _isHandleFailed = false;
            _countHandleFailed = -1;
        }

       

        private Queue(int threadCount, AbstractTaskExecuter executer, bool isHandleFailed, int countHandleFailed)
        {
            ThreadCount = threadCount;
            QueueTasks = new QueueTask[threadCount];
            Executer = executer;
            _isHandleFailed = isHandleFailed;
            _countHandleFailed = countHandleFailed;
            _taskStore = new TaskStore();

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
                executeTask.QueueTaskStatus = QueueTaskStatus.InProcces;
                 
                new TaskFactory().StartNew(() =>
                {
                    try
                    {
                        executeTask.ExecutedTask += Task_ExecutedTask;
                        Executer.Execute(executeTask);
                    }
                    catch 
                    {
                        executeTask.SetFailed();
                    }

                });
            }
            
        }

        public int AddTask(QueueTask queueTask)
        {
            _taskStore.Add(queueTask);

            TryStartTask();
           
            return _taskStore.IndexOf(queueTask);
        }

        /// <summary>
        /// Check executeble queueTask and return boolean value and queueTask index.
        /// </summary>
        /// <param name="isCan"></param>
        /// <param name="index"></param>
        private void IsCanExecuteTask(out bool isCan, out int index)
        {
            index = -1;
            int countExecuteTasks = QueueTasks.Count(each => CheckQueueTaskStatus(each));

            var task = _taskStore.FirstOrDefault(each => CheckQueueTaskStatus(each ));
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
            //if (task == null)
            //    return true;

            if (_isHandleFailed)
                return task?.QueueTaskStatus == QueueTaskStatus.Created || task.QueueTaskStatus == QueueTaskStatus.Failed;
            else
                return task?.QueueTaskStatus == QueueTaskStatus.Created;
        }

        private bool CheckTaskOnAttemptLimit(QueueTask task)
        {
            if (task == null || _isHandleFailed == false)
                return true;
            if (task.QueueTaskStatus == QueueTaskStatus.Failed && task.Attempt >= _countHandleFailed)
                return false;
            else
            {
                return false;
            }
        }

        #region Factory methods

        /// <summary>
        /// Create Queue which the try handle failed task n times.
        /// </summary>
        /// <param name="threadCount"></param>
        /// <param name="executer"></param>
        /// <param name="isHandleFailed"></param>
        /// <param name="countHandleFailed"></param>
        /// <returns></returns>
        public static Queue CreateQueueHandleFailed(int threadCount, AbstractTaskExecuter executer, bool isHandleFailed,
            int countHandleFailed)
        {
            return new Queue(threadCount, executer, isHandleFailed, countHandleFailed);
        }

        /// <summary>
        /// Create Queue.
        /// </summary>
        /// <param name="threadCount"></param>
        /// <param name="executer"></param>
        /// <returns></returns>
        public static Queue CreateQueue(int threadCount, AbstractTaskExecuter executer)
        {
            return new Queue(threadCount, executer);
        }

        public event Action<QueueTask> ExecutedTask;

        #endregion
    }
}
