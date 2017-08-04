using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractQueue
{
  public  class Queue
    {
        private readonly int ThreadCount;
        private readonly Task[] Tasks;
        private readonly AbstractTaskExecuter Executer;
        private readonly Store store;

        public Queue(int threadCount, AbstractTaskExecuter executer)
        {
            ThreadCount = threadCount;
            Tasks = new Task[threadCount];
            Executer = executer;
            store = new Store();
        }

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

        public int Add(Task task)
        {
            store.Add(task);
            TryStartTask();
            return store.IndexOf(task);
        }

        private void IsCanExecuteTask(out bool isCan, out int index)
        {
            index = -1;
            int countExecuteTasks = store.Count(each => each?.ETaskStatus == ETaskStatus.InProcces);
            var task = store.FirstOrDefault(each => each.ETaskStatus == ETaskStatus.Created);
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
