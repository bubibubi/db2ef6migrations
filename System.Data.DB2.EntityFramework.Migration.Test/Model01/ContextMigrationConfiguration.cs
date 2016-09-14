using System;
using System.Data.DB2.EntityFramework.Migrations;
using System.Data.Entity.Migrations;


namespace System.Data.DB2.EntityFramework.Migration.Test.Model01
{
    internal sealed class ContextMigrationConfiguration : DbMigrationsConfiguration<Context>
    {
        public ContextMigrationConfiguration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            SetSqlGenerator("IBM.Data.DB2", new DB2MigrationSqlGenerator());
            SetHistoryContextFactory("IBM.Data.DB2", (connection, defaultSchema) => new DB2HistoryContext(connection, defaultSchema));
        }
    }
}