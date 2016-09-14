using System;

namespace System.Data.DB2.EntityFramework.Migrations
{
    static class DB2ProviderManifestHelper
    {
        public const int MaxObjectNameLength = 255;

        /// <summary>
        /// Quotes an identifier
        /// </summary>
        /// <param name="name">Identifier name</param>
        /// <returns>The quoted identifier</returns>
        internal static string QuoteIdentifier(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            // DB2 is case sensitive so we must create objects in upper case
            return "\"" + name.ToUpper().Replace("\"", "\"\"") + "\"";
        }
    }
}
