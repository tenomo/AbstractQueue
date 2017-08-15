 using System.Threading.Tasks;
 using AbstractQueue.QueueData.Entities;

namespace AbstractQueue.Core
{
    public interface IQueue
    {
        int AttemptMaxCount { get; }
        string QueueName { get; set; }
        int AddTask(QueueTask queueTask);
          Task<int>  AddTaskAsync(QueueTask queueTask);
    }
}