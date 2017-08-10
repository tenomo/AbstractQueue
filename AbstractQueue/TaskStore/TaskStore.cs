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
    internal sealed class TaskStore : ITaskStore, ITaskExecutionObserve
    {
        private  QueueDataBaseContext _qdbContex;

        private QueueDataBaseContext QdbContex
        {
            get
            {
                if (_qdbContex == null)
                    _qdbContex = new QueueDataBaseContext(Config.ConnectionStringName);
                return _qdbContex;
            }
            set { _qdbContex = value; }
        }

        public event Action<QueueTask> SuccessExecuteTaskEvent;
        public event Action<QueueTask> FailedExecuteTaskEvent;
        public event Action<QueueTask> InProccesTaskEvent;
        private IQueryable<QueueTask> QueueTasks => QdbContex.QueueTasks;

        public void Add(QueueTask item)
        {
             var context = new  QueueDataBaseContext(Config.ConnectionStringName);
            QdbContex.QueueTasks.Add(item);
            QdbContex.SaveChanges();
        }

        public void Clear()
        {
            QdbContex.QueueTasks.ToList().Clear();
            QdbContex.SaveChanges();
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

        internal TaskStore()
        {
            _qdbContex = new QueueDataBaseContext(Config.ConnectionStringName);
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
            return QdbContex.QueueTasks.ToList();
        }

        public QueueTask GetById(int id)
        {
            return QdbContex.QueueTasks.Find(id);
        }

        private QueueTask GetById(int id, QueueDataBaseContext QdbContex)
        {
            return QdbContex.QueueTasks.Find(id);
        }

        public QueueTask GetById(string id)
        {
            return QdbContex.QueueTasks.Find(id);
        }

        public QueueTask Get(QueueTask entity)
        {
            return QdbContex.QueueTasks.Find(entity);
        }

        public void Update(QueueTask entity)
        { 
            var task = GetById(entity.Id, QdbContex);
            if (task == null) return;
            task.QueueTaskStatus = entity.QueueTaskStatus;
            task.Body = entity.Body;
            task.Type = entity.Type;
            task.Attempt = entity.Attempt;
            task.CreationDate = entity.CreationDate;
            task.QueueName = entity.QueueName;
            task.TaskIndexInQueue = entity.TaskIndexInQueue;
            task.ExecutedDate = entity.ExecutedDate;
            QdbContex.SaveChanges();
        }

        public void DeleteById(int id)
        { 
            var task = GetById(id, QdbContex);
            QdbContex.QueueTasks.Remove(task);
            QdbContex.SaveChanges();
        }

        public IQueryable<QueueTask> FindBy(System.Linq.Expressions.Expression<Func<QueueTask, bool>> predicate)
        {
            return QdbContex.QueueTasks.Where(predicate);
        }

        public IQueryable<QueueTask> Where(System.Linq.Expressions.Expression<Func<QueueTask, bool>> predicate)
        {
            return QdbContex.QueueTasks.Where(predicate);
        }

        public QueueTask FirstOrDefault(System.Linq.Expressions.Expression<Func<QueueTask, bool>> predicate)
        { 

            return QdbContex.QueueTasks.FirstOrDefault(predicate);
        }

    }
}
