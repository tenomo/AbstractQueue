using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractQueue;
using AbstractQueue.QueueData;

namespace ConsoleApp1.Mocks
{
  public  class QueueDBContext : DbContext, IQueueDBContext
    {
        public DbSet<QueueTask> Tasks { get; set; }

        
       
    }
}
