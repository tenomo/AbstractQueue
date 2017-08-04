using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractQueue
{
 public  sealed  class TaskStore : IList<QueueTask>
    {
        private readonly List<QueueTask> tasks = new List<QueueTask>();

        public IEnumerator<QueueTask> GetEnumerator()
        {
            return tasks.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return tasks.GetEnumerator();
        }

        public void Add(QueueTask item)
        {
            tasks.Add(item);
        }

        public void Clear()
        {
            tasks.Clear();
        }

        public bool Contains(QueueTask item)
        {
            return tasks.Contains(item);
        }

        public void CopyTo(QueueTask[] array, int arrayIndex)
        {
            tasks.CopyTo(array, arrayIndex);
        }

        public bool Remove(QueueTask item)
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

        public int IndexOf(QueueTask item)
        {
            return tasks.IndexOf(item);
        }

        public void Insert(int index, QueueTask item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public QueueTask this[int index]
        {
            get { return tasks[index]; }
            set { tasks[index] = value; }

        }
    }
}
