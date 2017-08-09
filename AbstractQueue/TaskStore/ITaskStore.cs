﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AbstractQueue.TaskStore
{
    /// <summary>
    /// Task the provide thread-safe interface for work with queue database contex
    /// </summary>
    public interface ITaskStore
    {
        QueueTask this[int index] { get; set; }

        int Count { get; }
        bool IsReadOnly { get; }
        event Action<QueueTask> ExecutedTaskEvent;
        event Action<QueueTask> FailedExecuteTaskEvent;
        event Action<QueueTask> InProccesTaskEvent;
        void Add(QueueTask item);
        void Clear();
        bool Contains(QueueTask item);
        void CopyTo(QueueTask[] array, int arrayIndex);
        void DeleteById(int id); 
        IQueryable<QueueTask> FindBy(Expression<Func<QueueTask, bool>> predicate);
        QueueTask FirstOrDefault(Expression<Func<QueueTask, bool>> predicate);
        QueueTask Get(QueueTask entity);
        IList<QueueTask> GetAll();
        QueueTask GetById(int id);
        int IndexOf(QueueTask item);
        void SetFailedStatus(QueueTask task);
        void SetSuccessStatus(QueueTask task);
        void Update(QueueTask entity);
        IQueryable<QueueTask> Where(Expression<Func<QueueTask, bool>> predicate);
    }
}