using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractQueue.QueueData.Context
{
   internal static class DbContextPool
    {
       private static List<DbContextWrapper> pool = new List<DbContextWrapper>();

        internal static DbContextWrapper GetFreeDbContext()
        {
            
         var freeContext=   pool.FirstOrDefault(each => each.InProccess == false);
            if (freeContext == null)
            {
                pool.Add(new DbContextWrapper());
                return GetFreeDbContext();
            }

            
            freeContext.SetStatusBusy();
            return freeContext;

        }


        private static void Clear()
        {
            var freeDbContextWrappers = pool.Where(each => each.InProccess == false);

            if (freeDbContextWrappers.Count() > 10)
            {
              var excess =  freeDbContextWrappers.Take(freeDbContextWrappers.Count() - 10);

                foreach (var VARIABLE in excess)
                {
                    VARIABLE.QueueDataBaseContext.Database.Connection.Close();
                    VARIABLE.QueueDataBaseContext.Dispose();
                    pool.Remove(VARIABLE);
                }
            
            }

            Logger.Log("Clear pool: deleted: " + freeDbContextWrappers.Count() + " elements");
        }

        internal static void ReturnToPool(DbContextWrapper obj)
        { 
            obj.SetStatusFree();
            Clear();
        }


    }
}
