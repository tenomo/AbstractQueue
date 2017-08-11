using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractQueue.Infrastructure
{
   internal interface IBusy
    {
        bool InProccess { get;  }
        void SetStatusBusy();

        void SetStatusFree();
    }
}
