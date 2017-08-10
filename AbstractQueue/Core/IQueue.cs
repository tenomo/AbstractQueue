using System;
using AbstractQueue.QueueData.Entities;

namespace AbstractQueue.Core
{
    internal interface IQueue : TaskExecutionObaerve
    {
        int AttemptMaxCount { get; }
        string QueueName { get; set; }
         
       

        int AddTask(QueueTask queueTask);

    }
}