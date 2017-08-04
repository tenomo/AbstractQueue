using System;

namespace AbstractQueue
{
    public interface IExecutedTask
    {
        event Action<QueueTask> ExecutedTask;
    }
}