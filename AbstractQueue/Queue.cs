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
        /// Executeble task array.
        /// </summary>
        private readonly Task[] Tasks;
        /// <summary>
        /// Concrete executer
        /// </summary>
        private readonly AbstractTaskExecuter Executer;

        private readonly bool _isHandleFailed;
        private readonly int _countHandleFailed;

        /// <summary>
        /// Task _taskStore.
        /// </summary>
        private readonly TaskStore _taskStore;

        public Queue(int threadCount, AbstractTaskExecuter executer)
        {
            ThreadCount = threadCount;
            Tasks = new Task[threadCount];
            Executer = executer;
            _taskStore = new TaskStore();
            _isHandleFailed = false;
            _countHandleFailed = -1;
        }

       

        public Queue(int threadCount, AbstractTaskExecuter executer, bool isHandleFailed, int countHandleFailed)
        {
            ThreadCount = threadCount;
            Tasks = new Task[threadCount];
            Executer = executer;
            _isHandleFailed = isHandleFailed;
            _countHandleFailed = countHandleFailed;
            _taskStore = new TaskStore();

        }

        /// <summary>
        /// Executed task handler.
        /// </summary>
        /// <param name="task"></param>
        private void Task_ExecutedTask(Task task)
        {
            if (task.ETaskStatus == ETaskStatus.InProcces)
            {
                task.ETaskStatus = ETaskStatus.Success;
                task.ExecutedDate = DateTime.Now;
            }
            else if (task.ETaskStatus == ETaskStatus.NotProcces)
            {
                task.ETaskStatus = ETaskStatus.NotProcces;
            }

            TryStartTask();
        }


        private void TryStartTask()
        {
            bool isCan;
            int taskId;
            IsCanExecuteTask(out isCan, out taskId);
            if (isCan)
            {
                new TaskFactory().StartNew(() =>
                {

                    var executeTask = Tasks[taskId];
                    executeTask.ETaskStatus = ETaskStatus.InProcces;
                    executeTask.ExecutedTask += Task_ExecutedTask;
                    Executer.Execute(executeTask);
                });
            }
        }

        public int AddTask(Task task)
        {
            _taskStore.Add(task);
            TryStartTask();
            return _taskStore.IndexOf(task);
        }

        /// <summary>
        /// Check executeble task and return boolean value and task index.
        /// </summary>
        /// <param name="isCan"></param>
        /// <param name="index"></param>
        private void IsCanExecuteTask(out bool isCan, out int index)
        {
            index = -1;
            int countExecuteTasks = _taskStore.Count(each => each?.ETaskStatus == ETaskStatus.InProcces);
            var task = _taskStore.FirstOrDefault(each => each.ETaskStatus == ETaskStatus.Created);
            isCan = countExecuteTasks < ThreadCount && task != null;
            if (isCan)
                for (index = 0; index < Tasks.Length; index++)
                {
                    var each = Tasks[index];
                    if (each == null || each?.ETaskStatus == ETaskStatus.NotProcces)
                    {
                        Tasks[index] = task;
                        return;
                    }
                }
        }
    }
}
