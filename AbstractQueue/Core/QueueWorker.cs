using System;
using System.Linq;
using AbstractQueue.QueueData.Entities;

namespace AbstractQueue.Core
{
   internal class QueueWorker
    {
        internal int Id { get; set; }
        /// <summary>
        /// Count workers.
        /// </summary>
        private readonly int QueueWorkersCount;

         
        /// <summary>
        /// Concrete executer
        /// </summary>
        private readonly AbstractTaskExecuter Executer;

        private bool isTryHandleError = false;
        private int AttemptMaxCount;

        /// <summary>
        /// QueueTasks TaskStore.
        /// </summary>
        private volatile TaskStore.TaskStore TaskStore;

        /// <summary>
        /// Invoke at executed any task
        /// </summary>
        public event Action<QueueTask> ExecutedTaskEvent;

        /// <summary>
        /// Current queue name
        /// </summary>
        public string QueueName { get; set; }

        public int CountHandleFailed
        {
            get { return AttemptMaxCount; }
            private set
            {
                if (value > 1)
                    isTryHandleError = true;
                else if (value < 0)
                {
                    throw new ArgumentException("CountHandleFailed must be more 0");
                }

                AttemptMaxCount = value;
            }
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


        /// <summary>
        /// Try execute task.
        /// </summary>
        private void TryStartTask()
        {
            var isCan = false;
            var workerId = 0;
            IsCanExecuteTask(out isCan, out workerId);
            if (!isCan) return;

            var executeTask = QueueWorkers[workerId];
            UpAttempt(executeTask);
            TaskStore.SetProccesStatus(executeTask);

            new TaskFactory().StartNew(() =>
            {
                try
                {
                    Executer.Execute(executeTask);
                    TaskStore.SetSuccessStatus(executeTask);
                }
                catch
                {
                    TaskStore.SetFailedStatus(executeTask);
                }
            });
        }
        /// <summary>
        /// Try execute task.
        /// </summary>
        private void TryStartTask(int workerId)
        {
            bool isCan = false;
            IsCanExecuteTask(out isCan, workerId);
            if (!isCan) return;

            var executeTask = QueueWorkers[workerId];
            UpAttempt(executeTask);
            TaskStore.SetProccesStatus(executeTask);
            try
            {
                Executer.Execute(executeTask);
                TaskStore.SetSuccessStatus(executeTask);
            }
            catch
            {
                TaskStore.SetFailedStatus(executeTask);
            }
        }

        /// <summary>
        /// Check executeble queueTask and return boolean value and queueTask workerId.
        /// </summary>
        /// <param name="isCan"></param>
        /// <param name="index"></param>
        private void IsCanExecuteTask(out bool isCan, out int index)
        {
            index = 0;
            /*
                        isCan = false;
            */
            var queueWorkers = QueueWorkers;
            var countExecuteTasks = queueWorkers.Count(CheckQueueTaskStatus);
            var queueWorkerCount = QueueWorkersCount;

            var task = TaskStore.GetAll().FirstOrDefault(each => CheckQueueTaskStatus(each) && each.QueueName == QueueName);

            isCan = countExecuteTasks < queueWorkerCount && task != null;

            if (!isCan) return;

            for (var i = 0; i < queueWorkerCount; i++)
            {
                index = i;
                var currentWorker = queueWorkers[index];

                if (currentWorker != null && !CheckQueueTaskStatus(currentWorker) &&
                    !CheckTaskOnAttemptLimit(currentWorker)) continue;

                queueWorkers[index] = task;
                queueWorkers[index].TaskIndexInQueue = index;
                TaskStore.Update(task);
                return;
            }
        }

        /// <summary>
        /// Check executeble queueTask by on executed task place and return boolean.
        /// </summary>
        /// <param name="isCan"></param>
        /// <param name="index"></param>
        /// <param name="index"></param>
        private void IsCanExecuteTask(out bool isCan, int index)
        {
            var queueWorkers = QueueWorkers;
            var queueWorkerCount = QueueWorkersCount;
            /*
                        isCan = false;
            */
            int countExecuteTasks = queueWorkers.Count(CheckQueueTaskStatus);
            var task = TaskStore.GetAll().FirstOrDefault(each => CheckQueueTaskStatus(each) && each.QueueName == QueueName);

            isCan = countExecuteTasks < queueWorkerCount && task != null;
            if (!isCan) return;
            queueWorkers[index] = task;
            queueWorkers[index].TaskIndexInQueue = index;
            TaskStore.Update(task);
        }


        private bool CheckQueueTaskStatus(QueueTask task)
        {
            var _isHandleFailed = this.isTryHandleError;
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
            if (task == null || isTryHandleError == false)
                return true;
            return task.QueueTaskStatus == QueueTaskStatus.Failed && task.Attempt <= countHandleFailed;
        }

        /// <summary>
        /// Iterate the execution task attempt. 
        /// </summary>
        /// <param name="task"></param>
        private void UpAttempt(QueueTask task)
        {
            task.Attempt++;
            TaskStore.Update(task);
        }


    }
}
