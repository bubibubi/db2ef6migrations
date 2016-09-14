using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Text;

namespace System.Data.DB2.EntityFramework.Migrations
{
    class DB2DdlBuilder
    {
        private readonly StringBuilder _stringBuilder = new StringBuilder();

        public string GetCommandText()
        {
            return _stringBuilder.ToString();
        }


        public void AppendStringLiteral(string literalValue)
        {
            AppendSql("'" + literalValue.Replace("'", "''") + "'");
        }


        public void AppendIdentifier(string identifier)
        {
            string correctIdentifier;

            correctIdentifier = identifier.ToLower().StartsWith("dbo.") ? identifier.Substring(4) : identifier;

            if (correctIdentifier.Length > DB2ProviderManifestHelper.MaxObjectNameLength)
            {
                string guid = Guid.NewGuid().ToString().Replace("-", "");
                correctIdentifier = correctIdentifier.Substring(0, DB2ProviderManifestHelper.MaxObjectNameLength - guid.Length) + guid;
            }


            AppendSql(DB2ProviderManifestHelper.QuoteIdentifier(correctIdentifier));
        }


        public void AppendIdentifierList(IEnumerable<string> identifiers)
        {
            bool first = true;
            foreach (var identifier in identifiers)
            {
                if (first)
                    first = false;
                else
                    AppendSql(", ");
                AppendIdentifier(identifier);
            }
        }

        public void AppendType(EdmProperty column)
        {
            AppendType(column.TypeUsage, column.Nullable, column.TypeUsage.GetIsIdentity());
        }

        public void AppendType(TypeUsage typeUsage, bool isNullable, bool isIdentity)
        {

            bool isTimestamp = false;

            string db2TypeName = typeUsage.EdmType.Name;
            string db2Length = "";


            switch (db2TypeName)
            {
                case "decimal":
                case "numeric":
                    db2Length = string.Format(System.Globalization.CultureInfo.InvariantCulture, "({0}, {1})", typeUsage.GetPrecision(), typeUsage.GetScale());
                    break;
                case "binary":
                case "varbinary":
                case "varchar":
                case "char":
                    db2Length = string.Format("({0})", typeUsage.GetMaxLength());
                    break;
                default:
                    break;
            }

            AppendSql(db2TypeName);
            AppendSql(db2Length);
            AppendSql(isNullable ? " null" : " not null");

            if (isTimestamp)
#pragma warning disable 642
                ;// nothing to generate for identity
#pragma warning restore 642
            else if (isIdentity && db2TypeName == "guid")
                AppendSql(" default GenGUID()");
            else if (isIdentity)
               AppendSql(" GENERATED ALWAYS AS IDENTITY (START WITH 1 INCREMENT BY 1)");
        }

        /// <summary>
        /// Appends raw SQL into the string builder.
        /// </summary>
        /// <param name="text">Raw SQL string to append into the string builder.</param>
        public void AppendSql(string text)
        {
            _stringBuilder.Append(text);
        }

        /// <summary>
        /// Appends raw SQL into the string builder.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="p">The p.</param>
        public void AppendSql(string format, params object[] p)
        {
            _stringBuilder.AppendFormat(format, p);
        }


        /// <summary>
        /// Appends new line for visual formatting or for ending a comment.
        /// </summary>
        public void AppendNewLine()
        {
            _stringBuilder.Append("\r\n");
        }


        public string CreateConstraintName(string constraint, string objectName)
        {
            if (objectName.ToLower().StartsWith("dbo."))
                objectName = objectName.Substring(4);

            string name = string.Format("{0}_{1}", constraint, objectName);


            if (name.Length + 9 > DB2ProviderManifestHelper.MaxObjectNameLength)
                name = name.Substring(0, DB2ProviderManifestHelper.MaxObjectNameLength - 9);

            name += "_" + GetRandomString();

            return name;
        }

        // Returns an eigth nibbles string
        protected string GetRandomString()
        {
            Random random = new Random();
            string randomValue = "";
            for (int n = 0; n < 8; n++)
            {
                byte b = (byte)random.Next(15);
                randomValue += string.Format("{0:x1}", b);
            }

            return randomValue;
        }


    }
}
