using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractQueue;
using AbstractQueue.QueueData;

namespace AbstractQueueUnitTests.Mocks
{
    
  public  class QueueDBContext : DbContext, IQueueDBContext
    {
             public DbSet<QueueTask> Tasks { get; set; }

       
        public QueueDBContext() :base("name=DefaultConnection" )
        {
                
        }
    }

    public static class DBSingle
    {
        private static QueueDBContext _dbContext;

        public static QueueDBContext DbContext
        {
            get
            {
                if (_dbContext == null)
                    _dbContext = new QueueDBContext();
                return _dbContext;
            }
         
        }
    }
}
