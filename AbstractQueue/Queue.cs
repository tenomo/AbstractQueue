﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractQueue
{
  public  class Queue
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
                  executeTask.ExecutedTask += Task_ExecutedTask;
                    Executer.Execute(executeTask);
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
            int countExecuteTasks = QueueTasks.Count(each => CheckQueueTaskStatus(each, _isHandleFailed));

            var task = _taskStore.FirstOrDefault(each => CheckQueueTaskStatus(each , _isHandleFailed));
            isCan = countExecuteTasks < ThreadCount && task != null;
            if (isCan)
                for (index = 0; index < ThreadCount; index++)
                {
                    var currentWorker = QueueTasks[index];
                    if (currentWorker == null || currentWorker?.QueueTaskStatus == QueueTaskStatus.Created)
                    {
                        QueueTasks[index] = task;
                        return;
                    }
                }
        }

        private bool CheckQueueTaskStatus(QueueTask task, bool isHandleFailed )
        {
            //if (task == null)
            //    return true;

            if (isHandleFailed)
                return task?.QueueTaskStatus == QueueTaskStatus.Created || task.QueueTaskStatus == QueueTaskStatus.Failed;
            else
                return task?.QueueTaskStatus == QueueTaskStatus.Created;
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
        #endregion
    }
}
