using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Database2Sharp
{
    class Mssql2Sharp : Db2Sharp
    {
        public Mssql2Sharp(string dbname, string user, string password)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder()
            {
                DataSource = @"LocalDb)\MSSQLLocalDB",
                InitialCatalog = dbname,
                AttachDBFilename="",
                UserID = user,
                Password = password,
                Pooling = true
            };
            ConnectionString = builder.ConnectionString;
        }
    }
}
