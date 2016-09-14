using System;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;

namespace System.Data.DB2.EntityFramework.Migration.Test.Model01
{
    class Test
    {
        public static void Run(DbConnection connection)
        {
            using (var context = new Context(connection))
            {
                context.Entities.Add(new Entity() {Description = "Test"});
                context.SaveChanges();
            }

            using (var context = new Context(connection))
            {
                Debug.Assert(context.Entities.Any());
            }

        }
    }
}
