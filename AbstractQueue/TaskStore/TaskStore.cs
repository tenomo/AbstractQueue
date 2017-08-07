using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AbstractQueue.QueueData;

namespace AbstractQueue.TaskStore
{
    internal sealed class TaskStore : IList<QueueTask>, ITaskStore
    {
        private readonly IQueueDBContext context;

        private List<QueueTask> Tasks => context.Tasks.ToList();

        public IEnumerator<QueueTask> GetEnumerator() => Tasks.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Tasks.GetEnumerator();

        public void Add(QueueTask item)
        {
            context.Tasks.Add(item);
            SaveChanges();
        }

        public void Clear()
        {
            Tasks.Clear();
            context.SaveChanges();
        }

        public bool Contains(QueueTask item) => Tasks.Contains(item);
        public void CopyTo(QueueTask[] array, int arrayIndex) => Tasks.CopyTo(array, arrayIndex);
        public bool Remove(QueueTask item) => Tasks.Remove(item);
        public int Count => Tasks.Count;
        public bool IsReadOnly => false;
        public int IndexOf(QueueTask item) => Tasks.IndexOf(item);
        public void Insert(int index, QueueTask item) => Tasks.Insert(index, item);
        public void RemoveAt(int index) => Tasks.RemoveAt(index);

        public QueueTask this[int index]
        {
            get { return Tasks[index]; }
            set { Tasks[index] = value; }
        }

        internal TaskStore(IQueueDBContext context)
        {
            this.context = context;
            ExecutedTaskEvent += TaskStore_SetStatus;
            FailedExecuteTaskEvent += TaskStore_SetStatus;
            InProccesTaskEvent += TaskStore_SetStatus;
        }

        private void TaskStore_SetStatus(QueueTask obj)
        {
            SaveChanges();
        }

        public int SaveChanges()=> context.SaveChanges();

        public void SetFailed(QueueTask task)
        {
             task.QueueTaskStatus = QueueTaskStatus.Failed;
         FailedExecuteTaskEvent?.Invoke(task);
            
        }

        public void SetSuccess(QueueTask task)
        {
            
                task.QueueTaskStatus = QueueTaskStatus.Success;
          //  task.ExecutedDate = DateTime.Now;
            ExecutedTaskEvent?.Invoke(task);
             
        }

        internal void SetProcces(QueueTask task)
        {

            task.QueueTaskStatus = QueueTaskStatus.InProcces;

            InProccesTaskEvent?.Invoke(task);


        }



        public event Action<QueueTask> ExecutedTaskEvent ;
        public event Action<QueueTask> FailedExecuteTaskEvent;
        public event Action<QueueTask> InProccesTaskEvent;

    }
}
