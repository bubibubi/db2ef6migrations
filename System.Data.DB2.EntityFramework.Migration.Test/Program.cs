using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.DB2.EntityFramework.Migration.Test.Model01;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBM.Data.DB2;

namespace System.Data.DB2.EF6.Migration
{
    class Program
    {
        static void Main(string[] args)
        {
            DbConnection connection = new DB2Connection("Server=10.0.0.51:50000;Database=SAMPLE;UID=Administrator;PWD=dacambiare;CurrentSchema=DBO");


            Test.Run(connection);

        }
    }
}
