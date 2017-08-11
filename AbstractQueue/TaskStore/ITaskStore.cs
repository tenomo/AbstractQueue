using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AbstractQueue.Infrastructure;
using AbstractQueue.QueueData.Entities;

namespace AbstractQueue.TaskStore
{
    /// <summary>
    /// Task the provide thread-safe interface for work with queue database contex
    /// </summary>
    public interface ITaskStore  
    {
        QueueTask this[int index] { get; set; }
        string QueueName { get;   }
        string Id { get; }
        void Add(QueueTask item);
        void Clear();
        void DeleteById(string id);
        QueueTask FirstOrDefault(Expression<Func<QueueTask, bool>> predicate);
        IList<QueueTask> GetAll();
        int IndexOf(QueueTask item);
        void SetFailedStatus(QueueTask task);
        void SetSuccessStatus(QueueTask task);
        void Update(QueueTask entity);
        IEnumerable<QueueTask> Where(Expression<Func<QueueTask, bool>> predicate);
    }
}