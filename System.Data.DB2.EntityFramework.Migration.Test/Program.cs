using System;
using System.Data.Common;
using System.Data.DB2.EntityFramework.Migration.Test.Model01;
using IBM.Data.DB2;

namespace System.Data.DB2.EF6.Migration
{
    class Program
    {
        static void Main(string[] args)
        {
            DbConnection connection = new DB2Connection("Server=10.0.0.51:50000;Database=SAMPLE;UID=Administrator;PWD=***;CurrentSchema=DBO");


            Test.Run(connection);

        }
    }
}
