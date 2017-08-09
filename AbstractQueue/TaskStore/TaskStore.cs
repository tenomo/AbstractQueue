using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AbstractQueue.QueueData;

namespace AbstractQueue.TaskStore
{
    internal sealed class TaskStore : ITaskStore// :   ITaskStore
    {



        private QueueDataBaseContext QDBContex { get; set; }

        private IQueryable<QueueTask> QueueTasks => QDBContex.QueueTask;



        public void Add(QueueTask item)
        {
            new TaskFactory().StartNew(() => {
                var context = new QueueDataBaseContext();
                context.QueueTask.Add(item);
                context.SaveChanges();
               // context.Database.Connection.Close();
            }).Wait();

        }


        public void Clear()
        {
           
            QDBContex.QueueTask.ToList().Clear();
            QDBContex.SaveChanges();
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
            ExecutedTaskEvent += TaskStore_SetStatus;
            FailedExecuteTaskEvent += TaskStore_SetStatus;
            InProccesTaskEvent += TaskStore_SetStatus;
            QDBContex  = new QueueDataBaseContext();
        }

        private void TaskStore_SetStatus(QueueTask obj)
        {
        Update(obj);
        }
        

        public void SetFailed(QueueTask task)
        {
             task.QueueTaskStatus = QueueTaskStatus.Failed;
             
            task.ExecutedDate = DateTime.Now;
            Update(task);
            FailedExecuteTaskEvent?.Invoke(task);
            
        }

        public void SetSuccess(QueueTask task)
        {
            
                task.QueueTaskStatus = QueueTaskStatus.Success;
              
                task.ExecutedDate = DateTime.Now;
            Update(task);
                ExecutedTaskEvent?.Invoke(task);
             
      
             
        }

        internal void SetProcces(QueueTask task)
        {
             

                task.QueueTaskStatus = QueueTaskStatus.InProcces;
        Update(task);
            InProccesTaskEvent?.Invoke(task);

           
        }
         
         

        public IList<QueueTask> GetAll()
        {
            return QDBContex.QueueTask.ToList();
        }

        public QueueTask GetById(int id)
        {
            return QDBContex.QueueTask.Find(id);
        }

        public QueueTask GetById(string id)
        {
            return QDBContex.QueueTask.Find(id);
        }

        public QueueTask Get(QueueTask entity)
        {
            return QDBContex.QueueTask.Find(entity);
        }

       

        public void Update(QueueTask entity)
        {
            bool saveFailed;
            do
            {
                saveFailed = false;
                try
                {
                    var task = QDBContex.QueueTask.FirstOrDefault(each => each.Id == entity.Id);
                    if (task != null)
                    {
                        task.QueueTaskStatus = entity.QueueTaskStatus;
                        task.Body = entity.Body;
                        task.Type = entity.Type;
                        task.Attempt = entity.Attempt;
                        task.CreationDate = entity.CreationDate;
                        task.QueueName = entity.QueueName;
                        task.TaskIndexInQueue = entity.TaskIndexInQueue;
                        task.ExecutedDate = entity.ExecutedDate;
                        QDBContex.SaveChanges();
                    }
                     
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;

                    // Update original values from the database 
                    var entry = ex.Entries.Single();
                    entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                }

            } while (saveFailed);
        }


     

        public void Delete(QueueTask entity)
        {
           
            QDBContex.QueueTask.Remove(entity);
            QDBContex.SaveChanges();
        }

        public void DeleteById(int id)
        {
           
            var entityToRemove = GetById(id);

            QDBContex.QueueTask.Remove(entityToRemove);
            QDBContex.SaveChanges();
        }

     

        public IQueryable<QueueTask> FindBy(System.Linq.Expressions.Expression<Func<QueueTask, bool>> predicate)
        {
            var query = QDBContex.QueueTask.Where(predicate);

            return query;
        }

        public IQueryable<QueueTask> Where(System.Linq.Expressions.Expression<Func<QueueTask, bool>> predicate)
        {
            var query = QDBContex.QueueTask.Where(predicate);
            return query;
        }

        public QueueTask FirstOrDefault(System.Linq.Expressions.Expression<Func<QueueTask, bool>> predicate)
        {
            var query = QDBContex.QueueTask.FirstOrDefault(predicate);
            return query;
        }

        //public async Task<QueueTask> FirstOrDefaultAsync(System.Linq.Expressions.Expression<Func<QueueTask, bool>> predicate)
        //{
        //    var query = await QDBContex.QueueTask.FirstOrDefaultAsync(predicate);
        //    return query;
        //}

        public bool Any(System.Linq.Expressions.Expression<Func<QueueTask, bool>> predicate)
        {
            return QDBContex.QueueTask.Any(predicate);
        }


        public event Action<QueueTask> ExecutedTaskEvent ;
        public event Action<QueueTask> FailedExecuteTaskEvent;
        public event Action<QueueTask> InProccesTaskEvent;

    }
}
