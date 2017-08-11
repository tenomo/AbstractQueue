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
       internal QueueDataBaseContext QueueDataBaseContext { get; private set; }

       public DbContextWrapper( )
       {
            QueueDataBaseContext = new QueueDataBaseContext(Config.ConnectionStringName);
            SetStatusFree();
       }

        public bool InProccess { get; private set; }
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
