using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Queue mySyppQueue = new Queue(3, new MySuperExecuter());

            for (int i = 0; i < 20; i++)
            {
                mySyppQueue.Add(new Task((int)SuperTaskTypes.MySuperTaskWrapper, Encoding.UTF8.GetBytes("Helo world queue. Handle task [#] " + i)));
            }

            Console.WriteLine();
          
        }

        //class MySuperTaskWrapper
        //{
        //   public string SuperMessage { get; set; }
        //   public  int SuperNumber { get; set; }
        //}
        class MySuperExecuter : AbstractTaskExecuter
        {
          public  override void Execute(Task task)
            {
                switch (task.TaskType)
                {
                    case ((int)SuperTaskTypes.MySuperTaskWrapper):
                        try
                        {
                            Console.WriteLine(Encoding.UTF8.GetString(task.TaskBody));
                            base.SetTaskStatus(task, TaskStatus.InProcces);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(Encoding.UTF8.GetString(task.TaskBody));
                            base.SetTaskStatus(task, TaskStatus.Failed);
                        }
                    
                        break;
                }
            }
             
        }

        enum SuperTaskTypes
        {
            MySuperTaskWrapper
        }


        #region Serealizer - deserealizer

        

        #endregion
    }

     

    #region Queue modul

    abstract class AbstractTaskExecuter
    {

        /// <summary>
        /// Execute task.
        /// </summary>
        /// <param name="task"></param>
        public abstract void Execute(Task task);

        /// <summary>
        /// Set task status after executed.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="status"></param>
        protected void SetTaskStatus(Task task, TaskStatus status)
        {
            task.TaskStatus = status;
        }
    }

    class Queue
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

        private void Task_ExecutedTask(Task obj)
        {
            if (obj.TaskStatus == TaskStatus.InProcces)
                obj.TaskStatus = TaskStatus.Success;
            else if (obj.TaskStatus == TaskStatus.NotProcces)
            {
                obj.TaskStatus = TaskStatus.NotProcces;
                
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
            int countExecuteTasks = store.Count(each => each?.TaskStatus == TaskStatus.InProcces);
            var task = store.FirstOrDefault(each => each.TaskStatus == TaskStatus.Created);
            isCan = countExecuteTasks < ThreadCount && task  != null;
            if (isCan)
                for (index = 0; index < Tasks.Length; index++)
                {
                    var each = Tasks[index];
                    if (each == null || each?.TaskStatus == TaskStatus.NotProcces)
                    { 
                        Tasks[index] = task;
                        return;
                    }
                }
        }
    }



    enum TaskStatus
    {
        Created,
        InProcces,
        Failed,
        Success,
        NotProcces = Failed | Success | InProcces
    }

    class Task
    {
        public int Id { get; set; }
        public TaskStatus TaskStatus { get; set; }
        public int TaskType { get; set; }
        public byte[] TaskBody { get; set; }
        public event Action<Task> ExecutedTask;


        public Task(  int taskType, byte[] taskBody)
        { 
            TaskStatus = TaskStatus.Created;
            TaskType = taskType;
            TaskBody = taskBody;    
        }
    }

    class Store : IList<Task>
    {
        private readonly List<Task> tasks = new List<Task>();

        public IEnumerator<Task> GetEnumerator()
        {
            return tasks.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return tasks.GetEnumerator();
        }

        public void Add(Task item)
        {
            tasks.Add(item);
        }

        public void Clear()
        {
            tasks.Clear();
        }

        public bool Contains(Task item)
        {
            return tasks.Contains(item);
        }

        public void CopyTo(Task[] array, int arrayIndex)
        {
            tasks.CopyTo(array, arrayIndex);
        }

        public bool Remove(Task item)
        {
            return tasks.Remove(item);
        }

        public int Count
        {
            get { return tasks.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public int IndexOf(Task item)
        {
           return tasks.IndexOf(item);
        }

        public void Insert(int index, Task item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public Task this[int index]
        {
            get { return tasks[index]; }
            set { tasks[index] = value; }

        }
    }

    #endregion
}
