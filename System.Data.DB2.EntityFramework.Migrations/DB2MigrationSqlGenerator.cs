using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.Migrations.Sql;
using System.Diagnostics;
using IBM.Data.DB2.EntityFramework;

namespace System.Data.DB2.EntityFramework.Migrations
{
    /// <summary>
    /// Migration Ddl generator for DB2
    /// </summary>
    public sealed class DB2MigrationSqlGenerator : MigrationSqlGenerator
    {

        const string BATCHTERMINATOR = ";\r\n";

        /// <summary>
        /// Initializes a new instance of the <see cref="DB2MigrationSqlGenerator"/> class.
        /// </summary>
        public DB2MigrationSqlGenerator(DbConnection connection)
        {
            DB2ProviderServices db2ProviderServices = new DB2ProviderServices();
            string manifestToken = db2ProviderServices.GetProviderManifestToken(connection);
            base.ProviderManifest = db2ProviderServices.GetProviderManifest(manifestToken);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DB2MigrationSqlGenerator"/> class.
        /// </summary>
        public DB2MigrationSqlGenerator(string manifestToken)
        {
            DB2ProviderServices db2ProviderServices = new DB2ProviderServices();
            base.ProviderManifest = db2ProviderServices.GetProviderManifest(manifestToken);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DB2MigrationSqlGenerator"/> class.
        /// </summary>
        public DB2MigrationSqlGenerator()
        {
            DB2ProviderServices db2ProviderServices = new DB2ProviderServices();
            base.ProviderManifest = db2ProviderServices.GetProviderManifest("DB2, 09.00.0000, 0, 0");
        }


        /// <summary>
        /// Converts a set of migration operations into database provider specific SQL.
        /// </summary>
        /// <param name="migrationOperations">The operations to be converted.</param>
        /// <param name="providerManifestToken">Token representing the version of the database being targeted.</param>
        /// <returns>
        /// A list of SQL statements to be executed to perform the migration operations.
        /// </returns>
        public override IEnumerable<MigrationStatement> Generate(IEnumerable<MigrationOperation> migrationOperations, string providerManifestToken)
        {
            List<MigrationStatement> migrationStatements = new List<MigrationStatement>();

            foreach (MigrationOperation migrationOperation in migrationOperations)
                migrationStatements.Add(GenerateStatement(migrationOperation));
            return migrationStatements;
        }

        private MigrationStatement GenerateStatement(MigrationOperation migrationOperation)
        {
            MigrationStatement migrationStatement = new MigrationStatement();
            migrationStatement.BatchTerminator = BATCHTERMINATOR;
            migrationStatement.Sql = GenerateSqlStatement(migrationOperation);
            return migrationStatement;
        }

        private string GenerateSqlStatement(MigrationOperation migrationOperation)
        {
            dynamic concreteMigrationOperation = migrationOperation;
            return GenerateSqlStatementConcrete(concreteMigrationOperation);
        }

        private string GenerateSqlStatementConcrete(MigrationOperation migrationOperation)
        {
            Debug.Assert(false);
            return string.Empty;
        }


        #region History operations

        private string GenerateSqlStatementConcrete(HistoryOperation migrationOperation)
        {
            DB2DdlBuilder ddlBuilder = new DB2DdlBuilder();

            foreach (DbModificationCommandTree commandTree in migrationOperation.CommandTrees)
            {
                List<DbParameter> parameters;
                // Take care because here we have several queries so we can't use parameters...
                switch (commandTree.CommandTreeKind)
                {
                    case DbCommandTreeKind.Insert:
                        ddlBuilder.AppendSql(DB2DmlBuilder.GenerateInsertSql((DbInsertCommandTree)commandTree, out parameters, true));
                        break;
                    case DbCommandTreeKind.Delete:
                        ddlBuilder.AppendSql(DB2DmlBuilder.GenerateDeleteSql((DbDeleteCommandTree)commandTree, out parameters, true));
                        break;
                    case DbCommandTreeKind.Update:
                        ddlBuilder.AppendSql(DB2DmlBuilder.GenerateUpdateSql((DbUpdateCommandTree)commandTree, out parameters, true));
                        break;
                    case DbCommandTreeKind.Function:
                    case DbCommandTreeKind.Query:
                    default:
                        throw new InvalidOperationException(string.Format("Command tree of type {0} not supported in migration of history operations", commandTree.CommandTreeKind));
                }
                ddlBuilder.AppendSql(BATCHTERMINATOR);
            }

            return ddlBuilder.GetCommandText();

        }

        #endregion

        #region Move operations

        private string GenerateSqlStatementConcrete(MoveProcedureOperation migrationOperation)
        {
            throw new NotSupportedException("Move procedure not supported by DB2");
        }

        private string GenerateSqlStatementConcrete(MoveTableOperation migrationOperation)
        {
            throw new NotSupportedException("Move table not supported by DB2");
        }

        #endregion


        #region Procedure related operations
        private string GenerateSqlStatementConcrete(AlterProcedureOperation migrationOperation)
        {
            throw new NotSupportedException("Procedures are not supported by DB2 migration provider");
        }

        private string GenerateSqlStatementConcrete(CreateProcedureOperation migrationOperation)
        {
            throw new NotSupportedException("Procedures are not supported by DB2 migration provider");
        }


        private string GenerateSqlStatementConcrete(DropProcedureOperation migrationOperation)
        {
            throw new NotSupportedException("Procedures are not supported by DB2 migration provider");
        }


        private string GenerateSqlStatementConcrete(RenameProcedureOperation migrationOperation)
        {
            throw new NotSupportedException("Procedures are not supported by DB2 migration provider");
        }

        #endregion


        #region Rename operations


        private string GenerateSqlStatementConcrete(RenameColumnOperation migrationOperation)
        {
            DB2DdlBuilder ddlBuilder = new DB2DdlBuilder();
            ddlBuilder.AppendSql("ALTER TABLE ");
            ddlBuilder.AppendIdentifier(migrationOperation.Table);
            ddlBuilder.AppendSql(" RENAME COLUMN ");
            ddlBuilder.AppendIdentifier(migrationOperation.Name);
            ddlBuilder.AppendSql(" TO ");
            ddlBuilder.AppendIdentifier(migrationOperation.NewName);

            return ddlBuilder.GetCommandText();
        }

        private string GenerateSqlStatementConcrete(RenameIndexOperation migrationOperation)
        {
            DB2DdlBuilder ddlBuilder = new DB2DdlBuilder();
            ddlBuilder.AppendSql("RENAME INDEX ");
            ddlBuilder.AppendIdentifier(migrationOperation.Name);
            ddlBuilder.AppendSql(" TO ");
            ddlBuilder.AppendIdentifier(migrationOperation.NewName);

            return ddlBuilder.GetCommandText();
        }

        private string GenerateSqlStatementConcrete(RenameTableOperation migrationOperation)
        {
            DB2DdlBuilder ddlBuilder = new DB2DdlBuilder();

            ddlBuilder.AppendSql("RENAME TABLE ");
            ddlBuilder.AppendIdentifier(migrationOperation.Name);
            ddlBuilder.AppendSql(" TO ");
            ddlBuilder.AppendIdentifier(migrationOperation.NewName);

            return ddlBuilder.GetCommandText();
        }

        #endregion

        #region Columns
        private string GenerateSqlStatementConcrete(AddColumnOperation migrationOperation)
        {
            DB2DdlBuilder ddlBuilder = new DB2DdlBuilder();

            ddlBuilder.AppendSql("ALTER TABLE ");
            ddlBuilder.AppendIdentifier(migrationOperation.Table);
            ddlBuilder.AppendSql(" ADD COLUMN ");

            ColumnModel column = migrationOperation.Column;

            ddlBuilder.AppendIdentifier(column.Name);
            ddlBuilder.AppendSql(" ");
            TypeUsage storeType = ProviderManifest.GetStoreType(column.TypeUsage);
            ddlBuilder.AppendType(storeType, column.IsNullable ?? true, column.IsIdentity);
            ddlBuilder.AppendNewLine();


            return ddlBuilder.GetCommandText();
        }

        private string GenerateSqlStatementConcrete(DropColumnOperation migrationOperation)
        {
            DB2DdlBuilder ddlBuilder = new DB2DdlBuilder();

            ddlBuilder.AppendSql("ALTER TABLE ");
            ddlBuilder.AppendIdentifier(migrationOperation.Table);
            ddlBuilder.AppendSql(" DROP COLUMN ");

            ddlBuilder.AppendIdentifier(migrationOperation.Name);

            return ddlBuilder.GetCommandText();
        }

        private string GenerateSqlStatementConcrete(AlterColumnOperation migrationOperation)
        {
            DB2DdlBuilder ddlBuilder = new DB2DdlBuilder();

            ddlBuilder.AppendSql("ALTER TABLE ");
            ddlBuilder.AppendIdentifier(migrationOperation.Table);
            ddlBuilder.AppendSql(" ALTER COLUMN ");

            ColumnModel column = migrationOperation.Column;

            ddlBuilder.AppendIdentifier(column.Name);
            ddlBuilder.AppendSql(" ");
            TypeUsage storeType = ProviderManifest.GetStoreType(column.TypeUsage);
            ddlBuilder.AppendType(storeType, column.IsNullable ?? true, column.IsIdentity);
            ddlBuilder.AppendNewLine();


            return ddlBuilder.GetCommandText();
        }

        #endregion


        #region Foreign keys creation

        private string GenerateSqlStatementConcrete(AddForeignKeyOperation migrationOperation)
        {

            DB2DdlBuilder ddlBuilder = new DB2DdlBuilder();
            ddlBuilder.AppendSql("ALTER TABLE ");
            ddlBuilder.AppendIdentifier(migrationOperation.DependentTable);
            ddlBuilder.AppendSql(" ADD CONSTRAINT ");
            ddlBuilder.AppendIdentifier(migrationOperation.Name.Replace("dbo.", ""));
            ddlBuilder.AppendSql(" FOREIGN KEY (");
            ddlBuilder.AppendIdentifierList(migrationOperation.DependentColumns);
            ddlBuilder.AppendSql(")");
            ddlBuilder.AppendSql(" REFERENCES ");
            ddlBuilder.AppendIdentifier(migrationOperation.PrincipalTable);
            ddlBuilder.AppendSql(" (");
            ddlBuilder.AppendIdentifierList(migrationOperation.PrincipalColumns);
            ddlBuilder.AppendSql(")");
            
            if (migrationOperation.CascadeDelete)
                ddlBuilder.AppendSql(" ON DELETE CASCADE ");


            return ddlBuilder.GetCommandText();

        }

        #endregion

        #region Primary keys creation

        private string GenerateSqlStatementConcrete(AddPrimaryKeyOperation migrationOperation)
        {
            DB2DdlBuilder ddlBuilder = new DB2DdlBuilder();
            ddlBuilder.AppendSql("ALTER TABLE ");
            ddlBuilder.AppendIdentifier(migrationOperation.Table);
            ddlBuilder.AppendSql(" ADD CONSTRAINT ");
            ddlBuilder.AppendIdentifier(migrationOperation.Name.Replace("dbo.", ""));
            ddlBuilder.AppendSql(" PRIMARY KEY (");
            ddlBuilder.AppendIdentifierList(migrationOperation.Columns);
            ddlBuilder.AppendSql(")");
            return ddlBuilder.GetCommandText();
        }

        #endregion

        #region Table operations

        private string GenerateSqlStatementConcrete(AlterTableOperation migrationOperation)
        {
            /* 
             * DB2 does not support alter table
             * We should rename old table, create the new table, copy old data to new table and drop old table
            */

            throw new NotSupportedException("Alter column not supported by DB2");

        }

        private string GenerateSqlStatementConcrete(CreateTableOperation migrationOperation)
        {
            DB2DdlBuilder ddlBuilder = new DB2DdlBuilder();


            ddlBuilder.AppendSql("CREATE TABLE ");
            ddlBuilder.AppendIdentifier(migrationOperation.Name);
            ddlBuilder.AppendSql(" (");
            ddlBuilder.AppendNewLine();

            bool first = true;
            foreach (ColumnModel column in migrationOperation.Columns)
            {
                if (first)
                    first = false;
                else
                    ddlBuilder.AppendSql(",");

                ddlBuilder.AppendSql(" ");
                ddlBuilder.AppendIdentifier(column.Name);
                ddlBuilder.AppendSql(" ");
                TypeUsage storeTypeUsage = ProviderManifest.GetStoreType(column.TypeUsage);
                ddlBuilder.AppendType(storeTypeUsage, column.IsNullable ?? true, column.IsIdentity);
                ddlBuilder.AppendNewLine();
            }

            ddlBuilder.AppendSql(")");

            if (migrationOperation.PrimaryKey != null)
            {
                ddlBuilder.AppendSql(BATCHTERMINATOR);
                ddlBuilder.AppendSql(GenerateSqlStatementConcrete(migrationOperation.PrimaryKey));
            }

            return ddlBuilder.GetCommandText();
        }

        #endregion

        #region Index

        private string GenerateSqlStatementConcrete(CreateIndexOperation migrationOperation)
        {
            DB2DdlBuilder ddlBuilder = new DB2DdlBuilder();
            ddlBuilder.AppendSql("CREATE ");
            if (migrationOperation.IsUnique)
                ddlBuilder.AppendSql("UNIQUE ");
            ddlBuilder.AppendSql("INDEX ");
            ddlBuilder.AppendIdentifier(migrationOperation.Name);
            ddlBuilder.AppendSql(" ON ");
            ddlBuilder.AppendIdentifier(migrationOperation.Table);
            ddlBuilder.AppendSql(" (");
            ddlBuilder.AppendIdentifierList(migrationOperation.Columns);
            ddlBuilder.AppendSql(")");

            return ddlBuilder.GetCommandText();
        }

        #endregion

        #region Drop

        private string GenerateSqlStatementConcrete(DropForeignKeyOperation migrationOperation)
        {
            DB2DdlBuilder ddlBuilder = new DB2DdlBuilder();
            ddlBuilder.AppendSql("ALTER TABLE ");
            ddlBuilder.AppendIdentifier(migrationOperation.PrincipalTable);
            ddlBuilder.AppendSql(" DROP FOREIGN KEY ");
            ddlBuilder.AppendIdentifier(migrationOperation.Name);
            return ddlBuilder.GetCommandText();
        }

        private string GenerateSqlStatementConcrete(DropPrimaryKeyOperation migrationOperation)
        {
            DB2DdlBuilder ddlBuilder = new DB2DdlBuilder();
            ddlBuilder.AppendSql("ALTER TABLE ");
            ddlBuilder.AppendIdentifier(migrationOperation.Table);
            ddlBuilder.AppendSql(" DROP PRIMARY KEY ");
            return ddlBuilder.GetCommandText();
        }

        private string GenerateSqlStatementConcrete(DropIndexOperation migrationOperation)
        {
            DB2DdlBuilder ddlBuilder = new DB2DdlBuilder();
            ddlBuilder.AppendSql("DROP INDEX ");
            ddlBuilder.AppendIdentifier(migrationOperation.Name);
            return ddlBuilder.GetCommandText();
        }

        private string GenerateSqlStatementConcrete(DropTableOperation migrationOperation)
        {
            DB2DdlBuilder ddlBuilder = new DB2DdlBuilder();
            ddlBuilder.AppendSql("DROP TABLE ");
            ddlBuilder.AppendIdentifier(migrationOperation.Name);
            return ddlBuilder.GetCommandText();
        }

        #endregion

    }
}
