using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractQueue.QueueData.Entities;

namespace AbstractQueue.Core
{
   public interface TaskExecutionObaerve
    {
        event Action<QueueTask> SuccessExecuteTaskEvent;
        event Action<QueueTask> FailedExecuteTaskEvent;
        event Action<QueueTask> InProccesTaskEvent;
    }
}
