using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractQueue
{
 public   class TaskStore : IList<Task>
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
}
