**Project Description**
DB2 Entity Framework 6 Migrations


**How to use it**
 - Download the library (using NuGet search for _System.Data.DB2.EntityFramework.Migrations_)
 - Create a migration configuration
 - Setup the migration configuration (usually during first context creation)
 - Setup the history context (you have to change the name of migration history table from __MigrationHistory to a valid DB2 names because IBM DB2 EF provider does not quote names)

_Example_

{{

class Context : DbContext
{
    static Context()
    {
        Database.SetInitializer(new MigrateDatabaseToLatestVersion<Context, ContextMigrationConfiguration>(true));
    }

    // DbSets configuration

}

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

// We must add this class only to change the migration history table name
// We need to do it because the default migration history table name is __MigrationHistory and
//     1. DB2 .net entity framework provider does not quote object names
//     2. in DB2 _ is a reserved character at the beginning of object names
class DB2HistoryContext : HistoryContext
{
    public DB2HistoryContext(DbConnection dbConnection, string defaultSchema) 
        : base(dbConnection, defaultSchema) 
    { 
    } 
 
    protected override void OnModelCreating(DbModelBuilder modelBuilder) 
    { 
        base.OnModelCreating(modelBuilder); 
        modelBuilder.Entity<HistoryRow>().ToTable("MigrationHistory"); 
    } 
}

}}