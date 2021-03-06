﻿using System;
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
        [ThreadStatic]
        private static QueueDataBaseContext _qdbContex;
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

        private  QueueDataBaseContext QdbContex
        { 
            get
            {
                if (_qdbContex == null)
                    _qdbContex = new QueueDataBaseContext(Config.ConnectionStringName, Config.IsConnectionName);
                return _qdbContex;
            }
            set { _qdbContex = value; }
        }

        

        internal TaskStore(string queueName)
        {
            _qdbContex = new QueueDataBaseContext(Config.ConnectionStringName, Config.IsConnectionName);
            this.QueueName = queueName;
            Id = Guid.NewGuid().ToString().Substring(0, 10);


            Infrastructure.TaskExecutionObserver.Kernal.FailedExecuteTaskEvent += TaskStore_SetStatus;
            Infrastructure.TaskExecutionObserver.Kernal.SuccessExecuteTaskEvent += TaskStore_SetStatus;
            Infrastructure.TaskExecutionObserver.Kernal.InProccesTaskEvent += TaskStore_SetStatus;

        }

        public void Add(QueueTask item)
        { 
           
              //  using (var transaction = QdbContex.Database.BeginTransaction(System.Data.IsolationLevel.RepeatableRead  ))
              //  {
                    QdbContex.QueueTasks.Add(item);
                    QdbContex.SaveChanges();
                  //  transaction.Commit();
                //}
        }


        public void Update(QueueTask entity)
        { 
            //using (var transaction = QdbContex.Database.BeginTransaction(System.Data.IsolationLevel.RepeatableRead  ))
            //{
                var task = GetById(entity.Id);   
                if (entity.Id == task.Id)
                {
                    task = entity;
                    QdbContex.SaveChanges();
                //    transaction.Commit();
               // }
              }
        }

        public void Clear()
        {
            QdbContex.QueueTasks.ToList().Clear();
            QdbContex.SaveChanges();
        }

        public bool Contains(QueueTask item) => QdbContex.QueueTasks.Contains(item);
        public void CopyTo(QueueTask[] array, int arrayIndex) => QdbContex.QueueTasks.ToList().CopyTo(array, arrayIndex);
        public int Count => QdbContex.QueueTasks.ToList().Count;
        public bool IsReadOnly => false;
        public int IndexOf(QueueTask item) => QdbContex.QueueTasks.ToList().IndexOf(item);

        public QueueTask this[int index]
        {
            get { return QdbContex.QueueTasks.ToList()[index]; }
            set { QdbContex.QueueTasks.ToList()[index] = value; }
        }



        internal TaskStore()
        {

            Id = Guid.NewGuid().ToString().Substring(0, 10);
        }

        private void TaskStore_SetStatus(ITaskStore obj, QueueTask e)
        {
            if (obj.Id == this.Id)
                Update(e);
        }

        public void SetFailedStatus(QueueTask task)
        {
            //var test = QdbContex.QueueTasks.AsNoTracking()
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

        public   IQueryable<QueueTask> GetAll()
        {
            return QdbContex.QueueTasks;
        }

        public  QueueTask GetById(int id)
        {
            return QdbContex.QueueTasks.Find(id);
        }

       

        public  QueueTask GetById(string id)
        {
            return QdbContex.QueueTasks.Find(id);
        }

        public  QueueTask Get(QueueTask entity)
        {
            return QdbContex.QueueTasks.Find(entity);
        }
         
        public  void DeleteById(string id)
        {
            var task = GetById(id);
            QdbContex.QueueTasks.Remove(task);
            QdbContex.SaveChanges();
        }

        public  IEnumerable<QueueTask> FindBy(System.Linq.Expressions.Expression<Func<QueueTask, bool>> predicate)
        {

            return QdbContex.QueueTasks.Where(predicate).ToList();
        }

        public IQueryable<QueueTask> Where(System.Linq.Expressions.Expression<Func<QueueTask, bool>> predicate)
        {
            return QdbContex.QueueTasks.Where(predicate);
        }

        public  QueueTask FirstOrDefault(System.Linq.Expressions.Expression<Func<QueueTask, bool>> predicate)
        {
            return QdbContex.QueueTasks.FirstOrDefault(predicate);
        }

         
    }
}
