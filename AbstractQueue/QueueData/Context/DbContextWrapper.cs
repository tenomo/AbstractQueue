using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractQueue.Infrastructure;

namespace AbstractQueue.QueueData.Context
{
   internal class DbContextWrapper : IBusy
   {
       private bool _inProccess;
       internal QueueDataBaseContext QueueDataBaseContext { get; private set; }

       public DbContextWrapper( )
       {
            QueueDataBaseContext = new QueueDataBaseContext(Config.ConnectionStringName);
            SetStatusFree();
       }
        static object lockeObj = new object();
        public bool InProccess
       {
           get { 
               lock (lockeObj)
               {
                    return _inProccess;
               }
           }
           private set
            {
                lock (lockeObj)
                {
                      _inProccess = value;
                }
            }
       }

       public void SetStatusBusy()
        {
            InProccess = true;
        }

        public void SetStatusFree()
        {
            InProccess = false;
        }
    }
}
