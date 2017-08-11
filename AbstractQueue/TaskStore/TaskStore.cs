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
        static object lockObj = new object();
        public void Add(QueueTask item)
        {
            try
            {
                lock (lockObj)
                {
                     var dbcWrapper = DbContextPool.GetFreeDbContext();
                dbcWrapper.QueueDataBaseContext.QueueTasks.Add(item);
                dbcWrapper.QueueDataBaseContext.SaveChanges();
                dbcWrapper.QueueDataBaseContext.Database.Connection.Close();
                DbContextPool.ReturnToPool(dbcWrapper);
                }
               
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

       

        
        public QueueTask GetById(string id)
        {
            return QdbContex.QueueTasks.Find(id);
        }

       
        public void Update (QueueTask entity)
        {
            var dbcWrapper = DbContextPool.GetFreeDbContext();
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
            dbcWrapper.QueueDataBaseContext.SaveChanges();
            DbContextPool.ReturnToPool(dbcWrapper);
        }

        

         

        public void DeleteById(string id)
        {
            var task = GetById(id);
            QdbContex.QueueTasks.Remove(task);
            QdbContex.SaveChanges();
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
