using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDBContextWhichoutMigration
{
    class MySuperEntity_1
    {
        public int Id { get; set; }
        public int MySuperData { get; set; }
        public int MySuperMessage { get; set; }
        public int MySuperNumber { get; set; }
    }
    class MySuperDbContext_1 : DbContext
    {
        public DbSet<MySuperEntity_1> MySuperEntities { get; set; }

        public MySuperDbContext_1():base("name=DefaultConnection")
        {
            var DbInitializerStrategy = new CreateDatabaseIfNotExists<MySuperDbContext_1>();
            
            Database.SetInitializer<MySuperDbContext_1>(new CreateDatabaseIfNotExists<MySuperDbContext_1>());
           
        }
    }

    class MySuperEntity_2
    {
        public int Id { get; set; }
        public int MySuperData { get; set; }
        public int MySuperMessage { get; set; }
        public int MySuperNumber { get; set; }
    }

    class MySuperDbContext_2 : DbContext
    {
        public DbSet<MySuperEntity_2> MySuperEntities { get; set; }

        public MySuperDbContext_2() : base("name=DefaultConnection")
        {
            var DbInitializerStrategy = new CreateDatabaseIfNotExists<MySuperDbContext_2>();

            Database.SetInitializer<MySuperDbContext_2>(DbInitializerStrategy);

        }
    }
}
