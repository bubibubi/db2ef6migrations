using System;
using System.Data.Common;
using System.Data.Entity;

namespace System.Data.DB2.EntityFramework.Migration.Test.Model01
{
    class Context : DbContext
    {
        static Context()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<Context, ContextMigrationConfiguration>(true));
        }

        public Context(DbConnection connection) : base(connection, true)
        {
            
        }

        public DbSet<Entity> Entities { get; set; }

    }
}
