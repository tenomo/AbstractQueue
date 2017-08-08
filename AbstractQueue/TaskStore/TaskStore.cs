using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using AbstractQueue.QueueData;

namespace AbstractQueue.TaskStore
{
    internal sealed class TaskStore : ITaskStore// :   ITaskStore
    {
        object lockobj = new object();
        private IQueueDBContext _context;

        private IQueueDBContext context
        {
            get
            {
                lock (lockobj)
                {
                     return _context;
                }
               
            }
            set { _context = _context; }
        }

        private IQueryable<QueueTask> Tasks => context.Tasks;



        public void Add(QueueTask item)
        {
            lock (lockobj)
            {
                context.Tasks.Add(item);
                SaveChanges();
            }
        }


        public void Clear()
        {
            Tasks.ToList().Clear();
            context.SaveChanges();
        }

        public bool Contains(QueueTask item) => Tasks.Contains(item);
        public void CopyTo(QueueTask[] array, int arrayIndex) => Tasks.ToList().CopyTo(array, arrayIndex);
 
        public int Count => Tasks.ToList().Count;
        public bool IsReadOnly => false;
        public int IndexOf(QueueTask item) => Tasks.ToList().IndexOf(item);
 
   

        public QueueTask this[int index]
        {
            get { return Tasks.ToList()[index]; }
            set { Tasks.ToList()[index] = value; }
        }

        internal TaskStore(IQueueDBContext context)
        {
            this.context = context;
            ExecutedTaskEvent += TaskStore_SetStatus;
            FailedExecuteTaskEvent += TaskStore_SetStatus;
            InProccesTaskEvent += TaskStore_SetStatus;
        }

        private void TaskStore_SetStatus(QueueTask obj)
        {
            SaveChanges();
        }

        public int SaveChanges()=> context.SaveChanges();

        public void SetFailed(QueueTask task)
        {
             task.QueueTaskStatus = QueueTaskStatus.Failed;
            SaveChanges();
            task.ExecutedDate = DateTime.Now;
            FailedExecuteTaskEvent?.Invoke(task);
            
        }

        public void SetSuccess(QueueTask task)
        {
            lock (lockobj)
            {
                task.QueueTaskStatus = QueueTaskStatus.Success;
                SaveChanges();
                task.ExecutedDate = DateTime.Now;
                ExecutedTaskEvent?.Invoke(task);
            }
      
             
        }

        internal void SetProcces(QueueTask task)
        {
            lock (lockobj)
            {

                task.QueueTaskStatus = QueueTaskStatus.InProcces;
            SaveChanges();
            InProccesTaskEvent?.Invoke(task);

            }
        }
         
         

        public IList<QueueTask> GetAll()
        {
            return context.Tasks.ToList();
        }

        public QueueTask GetById(int id)
        {
            return context.Tasks.Find(id);
        }

        public QueueTask GetById(string id)
        {
            return context.Tasks.Find(id);
        }

        public QueueTask Get(QueueTask entity)
        {
            return context.Tasks.Find(entity);
        }

       

        public void Update(QueueTask entity)
        {
            bool saveFailed;
            do
            {
                saveFailed = false;
                try
                {
                    context.SaveChanges();
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
            //var entityToRemove = Get(entity);
            context.Tasks.Remove(entity);
            context.SaveChanges();
        }

        public void DeleteById(int id)
        {
            var entityToRemove = GetById(id);
            context.Tasks.Remove(entityToRemove);
            context.SaveChanges();
        }

        public void DeleteById(string id)
        {
            var entityToRemove = GetById(id);
            context.Tasks.Remove(entityToRemove);
            context.SaveChanges();
        }

        public IQueryable<QueueTask> FindBy(System.Linq.Expressions.Expression<Func<QueueTask, bool>> predicate)
        {
            var query = context.Tasks.Where(predicate);

            return query;
        }

        public IQueryable<QueueTask> Where(System.Linq.Expressions.Expression<Func<QueueTask, bool>> predicate)
        {
            var query = context.Tasks.Where(predicate);
            return query;
        }

        public QueueTask FirstOrDefault(System.Linq.Expressions.Expression<Func<QueueTask, bool>> predicate)
        {
            var query = context.Tasks.FirstOrDefault(predicate);
            return query;
        }

        public async Task<QueueTask> FirstOrDefaultAsync(System.Linq.Expressions.Expression<Func<QueueTask, bool>> predicate)
        {
            var query = await context.Tasks.FirstOrDefaultAsync(predicate);
            return query;
        }

        public bool Any(System.Linq.Expressions.Expression<Func<QueueTask, bool>> predicate)
        {
            return context.Tasks.Any(predicate);
        }


        public event Action<QueueTask> ExecutedTaskEvent ;
        public event Action<QueueTask> FailedExecuteTaskEvent;
        public event Action<QueueTask> InProccesTaskEvent;

    }
}
