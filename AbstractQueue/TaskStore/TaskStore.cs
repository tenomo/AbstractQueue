using System;
using System.Collections.Generic;
using System.Linq;
using AbstractQueue.Core;
using AbstractQueue.QueueData.Context;
using AbstractQueue.QueueData.Entities;

namespace AbstractQueue.TaskStore
{
    /// <summary>
    /// Task the provide thread-safe interface for work with queue database contex
    /// </summary>
    internal sealed class TaskStore : ITaskStore
    {
       
        private QueueDataBaseContext QDBContex => new QueueDataBaseContext();

        public event Action<QueueTask> SuccessExecuteTaskEvent;
        public event Action<QueueTask> FailedExecuteTaskEvent;
        public event Action<QueueTask> InProccesTaskEvent;
        private IQueryable<QueueTask> QueueTasks => QDBContex.QueueTasks;
        public void Add(QueueTask item)
        {
            
            var context = QDBContex;
            context.QueueTasks.Add(item);
            context.SaveChanges();
        }
        public void Clear()
        {
            var context = QDBContex;
            context.QueueTasks.ToList().Clear();
            context.SaveChanges();
        }
        public bool Contains(QueueTask item) => QueueTasks.Contains(item);
        public void CopyTo(QueueTask[] array, int arrayIndex) => QueueTasks.ToList().CopyTo(array, arrayIndex);
        public int Count => QueueTasks.ToList().Count;
        public bool IsReadOnly => false;
        public int IndexOf(QueueTask item) => QueueTasks.ToList().IndexOf(item);
        public QueueTask this[int index]
        {
            get { return QueueTasks.ToList()[index]; }
            set { QueueTasks.ToList()[index] = value; }
        }
        internal TaskStore( )
        { 
            SuccessExecuteTaskEvent += TaskStore_SetStatus;
            FailedExecuteTaskEvent += TaskStore_SetStatus;
            InProccesTaskEvent += TaskStore_SetStatus;
            
        }
        private void TaskStore_SetStatus(QueueTask obj)
        {
            Update(obj);
        }
        public void SetFailedStatus(QueueTask task)
        {
             task.QueueTaskStatus = QueueTaskStatus.Failed;
            task.ExecutedDate = DateTime.Now;
            Update(task);
            FailedExecuteTaskEvent?.Invoke(task);
            
        }
        public void SetSuccessStatus(QueueTask task)
        {
            task.QueueTaskStatus = QueueTaskStatus.Success;
            task.ExecutedDate = DateTime.Now;
            Update(task);
            SuccessExecuteTaskEvent?.Invoke(task);
        }
        internal void SetProccesStatus(QueueTask task)
        {
            task.QueueTaskStatus = QueueTaskStatus.InProcces;
            Update(task);
            InProccesTaskEvent?.Invoke(task);
        }
        public IList<QueueTask> GetAll()
        {
            return QDBContex.QueueTasks.ToList();
        }
        public QueueTask GetById(int id)
        {
            return QDBContex.QueueTasks.Find(id);
        }
        private QueueTask GetById(int id, QueueDataBaseContext context)
        {
            return context.QueueTasks.Find(id);
        }
        public QueueTask GetById(string id)
        {
            return QDBContex.QueueTasks.Find(id);
        }
        public QueueTask Get(QueueTask entity)
        {
            return QDBContex.QueueTasks.Find(entity);
        }

        public void Update(QueueTask entity)
        {
            var context = QDBContex;
            var task = GetById(entity.Id, context);
            if (task == null) return;
            task.QueueTaskStatus = entity.QueueTaskStatus;
            task.Body = entity.Body;
            task.Type = entity.Type;
            task.Attempt = entity.Attempt;
            task.CreationDate = entity.CreationDate;
            task.QueueName = entity.QueueName;
            task.TaskIndexInQueue = entity.TaskIndexInQueue;
            task.ExecutedDate = entity.ExecutedDate;
            context.SaveChanges(); 
        }
       
        public void DeleteById(int id) 
        {
            var context = QDBContex;
            var task = GetById(id, context);
            context.QueueTasks.Remove(task);
            context.SaveChanges();
        }

        public IQueryable<QueueTask> FindBy(System.Linq.Expressions.Expression<Func<QueueTask, bool>> predicate)
        { 
            return QDBContex.QueueTasks.Where(predicate);
        }
        public IQueryable<QueueTask> Where(System.Linq.Expressions.Expression<Func<QueueTask, bool>> predicate)
        {
            return QDBContex.QueueTasks.Where(predicate);
        }
        public QueueTask FirstOrDefault(System.Linq.Expressions.Expression<Func<QueueTask, bool>> predicate)
        { 
            return QDBContex.QueueTasks.FirstOrDefault(predicate);
        }
         
    }
}
