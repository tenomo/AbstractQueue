using System;
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

        public Queue(int threadCount, AbstractTaskExecuter executer)
        {
            ThreadCount = threadCount;
            QueueTasks = new QueueTask[threadCount];
            Executer = executer;
            _taskStore = new TaskStore();
            _isHandleFailed = false;
            _countHandleFailed = -1;
        }

       

        public Queue(int threadCount, AbstractTaskExecuter executer, bool isHandleFailed, int countHandleFailed)
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
            int countExecuteTasks = QueueTasks.Count(each => each?.QueueTaskStatus == QueueTaskStatus.InProcces);

           

            var task = _taskStore.FirstOrDefault(each => each.QueueTaskStatus == QueueTaskStatus.Created);
            isCan = countExecuteTasks < ThreadCount && task != null;
            if (isCan)
                for (index = 0; index < ThreadCount; index++)
                {
                    var each = QueueTasks[index];
                    if (each == null || each?.QueueTaskStatus == QueueTaskStatus.Created)
                    {
                        QueueTasks[index] = task;
                        return;
                    }
                }
        }

         
    }
}
