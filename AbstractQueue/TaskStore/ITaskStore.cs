using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AbstractQueue.TaskStore
{
   public   interface ITaskStore
    {
        QueueTask this[int index] { get; set; }

        int Count { get; }
        bool IsReadOnly { get; }

        event Action<QueueTask> ExecutedTaskEvent;
        event Action<QueueTask> FailedExecuteTaskEvent;
        event Action<QueueTask> InProccesTaskEvent;

        void Add(QueueTask item);
        bool Any(Expression<Func<QueueTask, bool>> predicate);
        void Clear();
        bool Contains(QueueTask item);
        void CopyTo(QueueTask[] array, int arrayIndex);
        void Delete(QueueTask entity);
        void DeleteById(int id); 
        IQueryable<QueueTask> FindBy(Expression<Func<QueueTask, bool>> predicate);
        QueueTask FirstOrDefault(Expression<Func<QueueTask, bool>> predicate);
        //Task<QueueTasks> FirstOrDefaultAsync(Expression<Func<QueueTasks, bool>> predicate);
        QueueTask Get(QueueTask entity);
        IList<QueueTask> GetAll();
        QueueTask GetById(int id);
        QueueTask GetById(string id);
        int IndexOf(QueueTask item);
      
         
        void SetFailed(QueueTask task);
        void SetSuccess(QueueTask task);
        void Update(QueueTask entity);
        IQueryable<QueueTask> Where(Expression<Func<QueueTask, bool>> predicate);
    }
}