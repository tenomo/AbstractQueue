using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDBContextWhichoutMigration
{
    class Program
    {
        static void Main(string[] args)
        {
            MySuperDbContext_1 superDbContext_1 = new MySuperDbContext_1();
            superDbContext_1.MySuperEntities.Add(new MySuperEntity_1()
            {
                MySuperData =  3,
                MySuperMessage =  4,
                MySuperNumber = 3
            });
            superDbContext_1.SaveChanges();


            MySuperDbContext_2 superDbContext_2 = new MySuperDbContext_2();
            superDbContext_2.MySuperEntities.Add(new MySuperEntity_2()
            {
                MySuperData = 3,
                MySuperMessage = 4,
                MySuperNumber = 3
            });
            superDbContext_2.SaveChanges();
            Console.ReadKey();
        }
    }
}
