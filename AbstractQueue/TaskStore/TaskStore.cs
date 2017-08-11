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
                //if (_qdbContex == null)
                //{
                    dbContextWrapper = DbContextPool.GetFreeDbContext();
                    _qdbContex = dbContextWrapper.QueueDataBaseContext;
              //  }

              //  return _qdbContex;
              //Logger.Log("DbContextPool.PoolSize: # "+  DbContextPool.PoolSize);
                return dbContextWrapper.QueueDataBaseContext;
            }
            
        }

        private IQueryable<QueueTask> QueueTasks
        {
            get
            {
                var tasks = QdbContex.QueueTasks;
                DbContextPool.ReturnToPool(dbContextWrapper);
                return tasks;

            }
        }




        internal TaskStore(string queueName)
        { 
            this.QueueName = queueName;
            Id = Guid.NewGuid().ToString().Substring(0, 10);
            Infrastructure.TaskExecutionObserver.Kernal.FailedExecuteTaskEvent += TaskStore_SetStatus;
            Infrastructure.TaskExecutionObserver.Kernal.SuccessExecuteTaskEvent += TaskStore_SetStatus;
            Infrastructure.TaskExecutionObserver.Kernal.InProccesTaskEvent += TaskStore_SetStatus;

        }

        internal TaskStore()
        {
      
            Id = Guid.NewGuid().ToString().Substring(0, 10);
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
                //Logger.Log("Exception at add new task: " + e.Message + "  " + isTaskNull + "  " + isQdbContexNull);
                throw e;
            }

        }

        public void Clear()
        {
            
            QdbContex.QueueTasks.ToList().Clear();
            QdbContex.SaveChanges();
            DbContextPool.ReturnToPool(dbContextWrapper);
        }

        
         
      
        
        public int IndexOf(QueueTask item) => QueueTasks.ToList().IndexOf(item);

        public QueueTask this[int index]
        {
            get
            {
                var res = QueueTasks.ToList()[index];
                DbContextPool.ReturnToPool(dbContextWrapper);
                return res;
            }
            set { QueueTasks.ToList()[index] = value; DbContextPool.ReturnToPool(dbContextWrapper); }
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
            var list = QdbContex.QueueTasks.ToList();
            DbContextPool.ReturnToPool(dbContextWrapper);
            return list;
        }

       

        
        public QueueTask GetById(string id)
        {
            var res = QdbContex.QueueTasks.Find(id);
            DbContextPool.ReturnToPool(dbContextWrapper);
            return res;
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
            var dbc = QdbContex;
                dbc.QueueTasks.Remove(task);
            dbc.SaveChanges();
            DbContextPool.ReturnToPool(dbContextWrapper);
        }

     

        public IEnumerable<QueueTask> Where(System.Linq.Expressions.Expression<Func<QueueTask, bool>> predicate)
        {
            var res = QdbContex.QueueTasks.Where(predicate).ToList();
            DbContextPool.ReturnToPool(dbContextWrapper);
            return res;
        }

        public QueueTask FirstOrDefault(System.Linq.Expressions.Expression<Func<QueueTask, bool>> predicate)
        {
            var res = QdbContex.QueueTasks.FirstOrDefault(predicate);
          DbContextPool.ReturnToPool( dbContextWrapper);
            return res;
        }

        ~TaskStore()
        {
            DbContextPool.ReturnToPool(dbContextWrapper);
        }
        
    }
}
