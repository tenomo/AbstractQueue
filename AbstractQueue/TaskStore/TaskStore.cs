using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using AbstractQueue.Core;
using AbstractQueue.Infrastructure;
using AbstractQueue.QueueData.Context;
using AbstractQueue.QueueData.Entities;

namespace AbstractQueue.TaskStore
{
    /// <summary>
    /// Task the provide thread-safe interface for work with queue database contex
    /// </summary>
    internal sealed class TaskStore : ITaskStore //, ITaskExecutionObserver
    {
        private QueueDataBaseContext _qdbContex;
        private string _id;
        private string queueName;

        public string QueueName
        {
            get { return queueName; }
            private set { queueName = value; }
        }

        public string Id
        {
            get { return _id; }
            private set { _id = value; }
        }


        private DbContextWrapper dbContextWrapper { get; set; }

        private QueueDataBaseContext QdbContex
        {
            get
            {
                if (_qdbContex == null)
                {
                    dbContextWrapper = DbContextPool.GetFreeDbContext();
                    _qdbContex = dbContextWrapper.QueueDataBaseContext;
                }

                return _qdbContex;
            }
            set { _qdbContex = value; }
        }
         
        private IQueryable<QueueTask> QueueTasks => QdbContex.QueueTasks;

        internal TaskStore(string queueName)
        { 
            this.QueueName = queueName;
            Id = Guid.NewGuid().ToString().Substring(0, 10);


            Infrastructure.TaskExecutionObserver.Kernal.FailedExecuteTaskEvent += TaskStore_SetStatus;
            Infrastructure.TaskExecutionObserver.Kernal.SuccessExecuteTaskEvent += TaskStore_SetStatus;
            Infrastructure.TaskExecutionObserver.Kernal.InProccesTaskEvent += TaskStore_SetStatus;

        }

        public void Add(QueueTask item)
        {
            try
            {
                QdbContex.QueueTasks.Add(item);
                QdbContex.SaveChanges();
                QdbContex.Database.Connection.Close();
            }
            catch (Exception e)
            {
                string isTaskNull = item == null ? "Task is null" : "Task: " + item.ToString();
                string isQdbContexNull = item == null ? "DB contex is null" : "";
                Logger.Log("Exception at add new task: " + e.ToString() + "  " + isTaskNull + "  " + isQdbContexNull);
                throw e;
            }

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
            Id = Guid.NewGuid().ToString().Substring(0, 10);
        }

        private void TaskStore_SetStatus(ITaskStore obj, QueueTask e)
        {
            if (obj.Id == this.Id)
                Update(e);
        }

        public void SetFailedStatus(QueueTask task)
        {
            task.QueueTaskStatus = QueueTaskStatus.Failed;
            task.ExecutedDate = DateTime.Now;
            Update(task); 
            Infrastructure.TaskExecutionObserver.Kernal.OnFailedExecuteTaskEvent(this, task);
        }

        public void SetSuccessStatus(QueueTask task)
        {
            task.QueueTaskStatus = QueueTaskStatus.Success;
            task.ExecutedDate = DateTime.Now;
            Update(task); 
            Infrastructure.TaskExecutionObserver.Kernal.OnSuccessExecuteTaskEvent(this, task);
        }

        internal void SetProccesStatus(QueueTask task)
        {
            task.QueueTaskStatus = QueueTaskStatus.InProcces;
            Update(task); 
            Infrastructure.TaskExecutionObserver.Kernal.OnInProccesTaskEvent(this, task);
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

        public void Update (QueueTask entity)
        {
            try
            {
                var task = GetById(entity.Id);
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
            catch (Exception e)
            {
                QdbContex.Database.Connection.Close();
                QdbContex.Dispose();
                var dbcWrapper = DbContextPool.GetFreeDbContext();
                QdbContex = dbcWrapper.QueueDataBaseContext;
                var task = GetById(entity.Id);
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
                DbContextPool.ReturnToPool(dbcWrapper);

            }
           
        }

         

        public void DeleteById(string id)
        {
            var task = GetById(id);
            QdbContex.QueueTasks.Remove(task);
            QdbContex.SaveChanges();
        }

        public IEnumerable<QueueTask> FindBy(System.Linq.Expressions.Expression<Func<QueueTask, bool>> predicate)
        {
            return QdbContex.QueueTasks.Where(predicate).ToList();
        }

        public IEnumerable<QueueTask> Where(System.Linq.Expressions.Expression<Func<QueueTask, bool>> predicate)
        {
            return QdbContex.QueueTasks.Where(predicate).ToList();
        }

        public QueueTask FirstOrDefault(System.Linq.Expressions.Expression<Func<QueueTask, bool>> predicate)
        {
            return QdbContex.QueueTasks.FirstOrDefault(predicate);
        }

        ~TaskStore()
        {
            DbContextPool.ReturnToPool(dbContextWrapper);
        }
        
    }
}
