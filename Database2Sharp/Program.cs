using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database2Sharp
{
    class Program
    {
        static void Main(string[] args)
        {
            TestTemplateClass ttc = new TestTemplateClass();
            dynamic t = new ExpandoObject();
            t.username = "username";
            t.password = "password";
            //ttc.sets<User>(t, "id=123");

            User user = new User() { username = "ahai", password = Encoding.Default.GetBytes("baomi"),create_time=DateTime.Now };
            ttc.add<User>(user);
            return;

            Mysql2Sharp mssql = new Mysql2Sharp("imbot", "root", "root");
            mssql.GetSchema();
            foreach(DbSchemaTable table in mssql.Tables)
            {
                DbModel  model = mssql.CreateModule(table);
                string klass = model.ToString();
                Console.WriteLine(klass);
                mssql.CreateInsertStatement(table);
                mssql.CreateSelectStatement(table);
                mssql.CreateSelectStatement1(table);
                mssql.CreateUpdateStatement(table);
                mssql.CreateDeleteStatement(table);
                mssql.CreateDeleteStatement1(table);
            }
            
            return;
        }
    }
}
