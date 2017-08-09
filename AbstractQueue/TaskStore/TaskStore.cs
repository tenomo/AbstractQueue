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
      


        private QueueDataBaseContext QDBContex => new QueueDataBaseContext();

        private IQueryable<QueueTask> QueueTasks => QDBContex.QueueTasks;



        public void Add(QueueTask item)
        {


            var context = QDBContex;
            context.QueueTasks.Add(item);
            context.SaveChanges();
                // context.Database.Connection.Close();
                //  }).Wait();

                //bool saveFailed;
                //do
                //{
                //    saveFailed = false;
                //    try
                //    {

                //        if (item != null)
                //        {

                //            context.SaveChanges();
                //            context.Database.Connection.Close();
                //        }

                //    }
                //    catch (DbUpdateConcurrencyException ex)
                //    {
                //        saveFailed = true;

                //        // Update original values from the database 
                //        var entry = ex.Entries.Single();
                //        entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                //    }

                //} while (saveFailed);
            
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
            ExecutedTaskEvent += TaskStore_SetStatus;
            FailedExecuteTaskEvent += TaskStore_SetStatus;
            InProccesTaskEvent += TaskStore_SetStatus;
            
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
            return QDBContex.QueueTasks.ToList();
        }

        public QueueTask GetById(int id)
        {
            return QDBContex.QueueTasks.Find(id);
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
            var conext = QDBContex;
            bool saveFailed;
            do
            {
                saveFailed = false;
                try
                {
                    
                    var task = conext.QueueTasks.FirstOrDefault(each => each.Id == entity.Id);
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
                        conext.SaveChanges();
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
           
            QDBContex.QueueTasks.Remove(entity);
            QDBContex.SaveChanges();
        }

        public void DeleteById(int id)
        {
           
            var entityToRemove = GetById(id);

            QDBContex.QueueTasks.Remove(entityToRemove);
            QDBContex.SaveChanges();
        }

     

        public IQueryable<QueueTask> FindBy(System.Linq.Expressions.Expression<Func<QueueTask, bool>> predicate)
        {
            var query = QDBContex.QueueTasks.Where(predicate);

            return query;
        }

        public IQueryable<QueueTask> Where(System.Linq.Expressions.Expression<Func<QueueTask, bool>> predicate)
        {
            var query = QDBContex.QueueTasks.Where(predicate);
            return query;
        }

        public QueueTask FirstOrDefault(System.Linq.Expressions.Expression<Func<QueueTask, bool>> predicate)
        {
            var query = QDBContex.QueueTasks.FirstOrDefault(predicate);
            return query;
        }

        //public async Task<QueueTasks> FirstOrDefaultAsync(System.Linq.Expressions.Expression<Func<QueueTasks, bool>> predicate)
        //{
        //    var query = await QDBContex.QueueTasks.FirstOrDefaultAsync(predicate);
        //    return query;
        //}

        public bool Any(System.Linq.Expressions.Expression<Func<QueueTask, bool>> predicate)
        {
            return QDBContex.QueueTasks.Any(predicate);
        }


        public event Action<QueueTask> ExecutedTaskEvent ;
        public event Action<QueueTask> FailedExecuteTaskEvent;
        public event Action<QueueTask> InProccesTaskEvent;

    }
}
