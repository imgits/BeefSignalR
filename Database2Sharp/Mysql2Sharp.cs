using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data.Common;
using System.IO;
using System.Reflection;

namespace Database2Sharp
{
    class Mysql2Sharp : Db2Sharp
    {
        public Mysql2Sharp(string dbname,string username, string password)
        {
            Init();
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder()
            {
                Server = "localhost",
                Port = 3306,
                Database = dbname,
                UserID = username,
                Password = password,
                Pooling = true
            };
            ConnectionString = builder.ConnectionString;
        }

        public override bool OpenConnection()
        {
            try
            {
                dbConnection = new MySqlConnection(ConnectionString);
                return base.OpenConnection();
            }
            catch(MySqlException ex)
            {
                Show(ex);
            }
            return false;
        }

        public override void Init()
        {
            DbConnectionTypeName = "MySqlConnection";
            DbCommandTypeName = "MySqlCommand";
            DbDataReaderTypeName = "MySqlDataReader";

            DbType2SharpType["int"] = "int";
            DbType2SharpType["varchar"] = "string";
            DbType2SharpType["text"] = "string";
            DbType2SharpType["datetime"] = "DateTime";
            DbType2SharpType["varbinary"] = "byte[]";
            DbType2SharpType["blob"] = "byte[]";
            DbType2SharpType["timestamp"] = "byte[]";
        }

        public override string GetSharpTypeName(string DbTypeName)
        {
            if (!DbType2SharpType.ContainsKey(DbTypeName)) return DbTypeName;
            return DbType2SharpType[DbTypeName];
        }

        public string CreateInsertStatement(DbSchemaTable table)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Database2Sharp.TemplateFile.cs");
            TextReader reader = new StreamReader(stream);

            string TemplateText = reader.ReadToEnd();
            int srart_pos = TemplateText.IndexOf("//InsertStatementStart");
            int end_pos = TemplateText.IndexOf("//InsertStatementEnd");
            srart_pos += "//InsertStatementStart".Length;
            string InsertTemplate = TemplateText.Substring(srart_pos, end_pos - srart_pos);
            
            string model = table.table_name;
            string Model = TableName2ModelName(model);
            string ColunmNameList="";
            string ColumnParamList="";
            string SetParameters="\n";
            int index = 0;
            foreach(DbSchemaColumn column in table.Columns)
            {
                string colname = column.column_name;
                if (colname == "id") continue;
                ColunmNameList += colname;
                ColumnParamList += "@" + colname;
                SetParameters += "\t\t\t";
                SetParameters += string.Format("cmd.Parameters.AddWithValue(\"@{0}\", {1}.{0});\n",colname, model);
                if (++index < table.Columns.Count-1)
                {
                    ColunmNameList += ',';
                    ColumnParamList += ',';
                }
            }
            string InsertCode = InsertTemplate.Replace("Model", Model);
            InsertCode = InsertCode.Replace("model", model);
            InsertCode = InsertCode.Replace("table_name", table.table_name);
            InsertCode = InsertCode.Replace("SqlConnection", DbConnectionTypeName);
            InsertCode = InsertCode.Replace("SqlCommand", DbCommandTypeName);
            InsertCode = InsertCode.Replace("ColunmNameList", ColunmNameList);
            InsertCode = InsertCode.Replace("ColumnParamList", ColumnParamList);
            InsertCode = InsertCode.Replace("//SetParameters", SetParameters);

            //Show(InsertTemplate);
            Show(InsertCode);
            return InsertCode;
        }

        public string CreateSelectStatement(DbSchemaTable table)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Database2Sharp.TemplateFile.cs");
            TextReader reader = new StreamReader(stream);
            string TemplateText = reader.ReadToEnd();

            string TemplateStart = "//SelectStatementStart";
            string TemplateEnd = "//SelectStatementEnd";
            int srart_pos = TemplateText.IndexOf(TemplateStart);
            int end_pos = TemplateText.IndexOf(TemplateEnd);
            srart_pos += TemplateStart.Length;
            string SelectTemplate = TemplateText.Substring(srart_pos, end_pos - srart_pos);

            string model = table.table_name;
            string Model = TableName2ModelName(model);
            string ReadColumns = "\n";
            string DefineStream = "";
            foreach (DbSchemaColumn column in table.Columns)
            {
                string colname = column.column_name;
                ReadColumns += "\t\t\t";
                switch (GetSharpTypeName(column.data_type))
                {
                    case "int":
                        ReadColumns += string.Format("{0}.{1} = reader.GetInt32(\"{1}\");\n", model, colname);
                        break;
                    case "string":
                        ReadColumns += string.Format("{0}.{1} = reader.GetString(\"{1}\");\n", model, colname);
                        break;
                    case "DateTime":
                        ReadColumns += string.Format("{0}.{1} = reader.GetDateTime(\"{1}\");\n", model, colname);
                        break;
                    case "byte[]":
                        DefineStream = "Stream st=null;";
                        ReadColumns += string.Format("st = reader.GetStream(\"{0}\");\n", colname);
                        ReadColumns += "\t\t\t" + string.Format("{0}.{1} = new byte[st.Length];\n", model, colname);
                        ReadColumns += "\t\t\t" + string.Format("st.Read({0}.{1},0,st.Length);\n", model, colname);
                        break;
                }
            }
            string SelectCode = SelectTemplate.Replace("Model", Model);
            SelectCode = SelectCode.Replace("model", model);
            SelectCode = SelectCode.Replace("table_name", table.table_name);
            SelectCode = SelectCode.Replace("SqlConnection", DbConnectionTypeName);
            SelectCode = SelectCode.Replace("SqlCommand", DbCommandTypeName);
            SelectCode = SelectCode.Replace("SqlDataReader", DbDataReaderTypeName);
            SelectCode = SelectCode.Replace("//DefineStream", DefineStream);
            SelectCode = SelectCode.Replace("//ReadColumns", ReadColumns);
            Show(SelectCode);
            return SelectCode;
        }

        public string CreateSelectStatement1(DbSchemaTable table)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Database2Sharp.TemplateFile.cs");
            TextReader reader = new StreamReader(stream);
            string TemplateText = reader.ReadToEnd();

            string TemplateStart = "//SelectStatement1Start";
            string TemplateEnd = "//SelectStatement1End";
            int srart_pos = TemplateText.IndexOf(TemplateStart);
            int end_pos = TemplateText.IndexOf(TemplateEnd);
            srart_pos += TemplateStart.Length;
            string SelectTemplate = TemplateText.Substring(srart_pos, end_pos - srart_pos);

            string model = table.table_name;
            string Model = TableName2ModelName(model);
            string ReadColumns = "\n";
            string DefineStream = "";
            foreach (DbSchemaColumn column in table.Columns)
            {
                string colname = column.column_name;
                ReadColumns += "\t\t\t";
                switch (GetSharpTypeName(column.data_type))
                {
                    case "int":
                        ReadColumns += string.Format("{0}.{1} = reader.GetInt32(\"{1}\");\n", model, colname);
                        break;
                    case "string":
                        ReadColumns += string.Format("{0}.{1} = reader.GetString(\"{1}\");\n", model, colname);
                        break;
                    case "DateTime":
                        ReadColumns += string.Format("{0}.{1} = reader.GetDateTime(\"{1}\");\n", model, colname);
                        break;
                    case "byte[]":
                        DefineStream = "Stream st=null;";
                        ReadColumns += string.Format("st = reader.GetStream(\"{0}\");\n", colname);
                        ReadColumns += "\t\t\t" + string.Format("{0}.{1} = new byte[st.Length];\n", model, colname);
                        ReadColumns += "\t\t\t" + string.Format("st.Read({0}.{1},0,st.Length);\n", model, colname);
                        break;
                }
            }
            string SelectCode = SelectTemplate.Replace("Model", Model);
            SelectCode = SelectCode.Replace("model", model);
            SelectCode = SelectCode.Replace("table_name", table.table_name);
            SelectCode = SelectCode.Replace("SqlConnection", DbConnectionTypeName);
            SelectCode = SelectCode.Replace("SqlCommand", DbCommandTypeName);
            SelectCode = SelectCode.Replace("SqlDataReader", DbDataReaderTypeName);
            SelectCode = SelectCode.Replace("//DefineStream", DefineStream);
            SelectCode = SelectCode.Replace("//ReadColumns", ReadColumns);
            Show(SelectCode);
            return SelectCode;
        }

        public string CreateUpdateStatement(DbSchemaTable table)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Database2Sharp.TemplateFile.cs");
            TextReader reader = new StreamReader(stream);
            string TemplateText = reader.ReadToEnd();

            string TemplateStart = "//UpdateStatementStart";
            string TemplateEnd = "//UpdateStatementEnd";
            int srart_pos =  TemplateText.IndexOf(TemplateStart);
            int end_pos = TemplateText.IndexOf(TemplateEnd);
            srart_pos += TemplateStart.Length;
            string InsertTemplate = TemplateText.Substring(srart_pos, end_pos - srart_pos);

            string model = table.table_name;
            string Model = TableName2ModelName(model);
            string NameValueList = "";
            string SetParameters = "\n";
            int index = 0;
            foreach (DbSchemaColumn column in table.Columns)
            {
                string colname = column.column_name;
                if (colname == "id") continue;
                NameValueList += colname +"=@" + colname;
                SetParameters += "\t\t\t";
                SetParameters += string.Format("cmd.Parameters.AddWithValue(\"@{0}\", {1}.{0});\n", colname, model);
                if (++index < table.Columns.Count - 1)
                {
                    NameValueList += ',';
                }
            }
            string UpdateCode = InsertTemplate.Replace("Model", Model);
            UpdateCode = UpdateCode.Replace("model", model);
            UpdateCode = UpdateCode.Replace("table_name", table.table_name);
            UpdateCode = UpdateCode.Replace("SqlConnection", DbConnectionTypeName);
            UpdateCode = UpdateCode.Replace("SqlCommand", DbCommandTypeName);
            UpdateCode = UpdateCode.Replace("NameValueList", NameValueList);
            UpdateCode = UpdateCode.Replace("//SetParameters", SetParameters);

            //Show(InsertTemplate);
            Show(UpdateCode);
            return UpdateCode;
        }

        public string CreateDeleteStatement(DbSchemaTable table)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Database2Sharp.TemplateFile.cs");
            TextReader reader = new StreamReader(stream);
            string TemplateText = reader.ReadToEnd();

            string TemplateStart = "//DeleteStatementStart";
            string TemplateEnd = "//DeleteStatementEnd";
            int srart_pos = TemplateText.IndexOf(TemplateStart);
            int end_pos = TemplateText.IndexOf(TemplateEnd);
            srart_pos += TemplateStart.Length;
            string DeleteTemplate = TemplateText.Substring(srart_pos, end_pos - srart_pos);

            string model = table.table_name;
            string Model = TableName2ModelName(model);

            string DeleteCode = DeleteTemplate.Replace("Model", Model);
            DeleteCode = DeleteCode.Replace("model", model);
            DeleteCode = DeleteCode.Replace("table_name", table.table_name);
            DeleteCode = DeleteCode.Replace("SqlConnection", DbConnectionTypeName);
            DeleteCode = DeleteCode.Replace("SqlCommand", DbCommandTypeName);

            Show(DeleteCode);
            return DeleteCode;
        }

        public string CreateDeleteStatement1(DbSchemaTable table)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Database2Sharp.TemplateFile.cs");
            TextReader reader = new StreamReader(stream);
            string TemplateText = reader.ReadToEnd();

            string TemplateStart = "//DeleteStatement1Start";
            string TemplateEnd = "//DeleteStatement1End";
            int srart_pos = TemplateText.IndexOf(TemplateStart);
            int end_pos = TemplateText.IndexOf(TemplateEnd);
            srart_pos += TemplateStart.Length;
            string DeleteTemplate = TemplateText.Substring(srart_pos, end_pos - srart_pos);

            string model = table.table_name;
            string Model = TableName2ModelName(model);

            string DeleteCode = DeleteTemplate.Replace("Model", Model);
            DeleteCode = DeleteCode.Replace("model", model);
            DeleteCode = DeleteCode.Replace("table_name", table.table_name);
            DeleteCode = DeleteCode.Replace("SqlConnection", DbConnectionTypeName);
            DeleteCode = DeleteCode.Replace("SqlCommand", DbCommandTypeName);

            Show(DeleteCode);
            return DeleteCode;
        }

    }
}
