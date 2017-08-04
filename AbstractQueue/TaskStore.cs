using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractQueue
{
    public sealed class TaskStore : IList<QueueTask>, ITaskStore
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

        public TaskStore(IQueueDBContext context)
        {
            this.context = context;
        }

        public int SaveChanges()=> context.SaveChanges();

        public void SetFailed(QueueTask task)
        {
            if (task.QueueTaskStatus == QueueTaskStatus.Failed || task.QueueTaskStatus == QueueTaskStatus.Success)
                ExecutedTask?.Invoke(task);
        }

        public void SetSuccess(QueueTask task)
        {
            if(task.QueueTaskStatus == QueueTaskStatus.Failed || task.QueueTaskStatus == QueueTaskStatus.Success)
                ExecutedTask?.Invoke(task);
        }

        public void SetProcces(QueueTask task)
        {
            if (task.QueueTaskStatus == QueueTaskStatus.Created)
                ExecutedTask?.Invoke(task);
            else
                throw new OperationCanceledException("The task is already completed");
        }

        public event Action<QueueTask> ExecutedTask;
    }
}
