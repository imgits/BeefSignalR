using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//https://msdn.microsoft.com/en-us/library/ms254969.aspx

namespace Database2Sharp
{

    class DbSchemaTable
    {
        Dictionary<string, string> items= new Dictionary<string, string>();
        List<DbSchemaColumn> columns = new List<DbSchemaColumn>();
        public void SetItem(string name, string value)
        {
            if (items.ContainsKey(name)) items[name] = value;
            else items.Add(name, value);
        }

        public void AddColumn(DbSchemaColumn column)
        {
            columns.Add(column);
        }

        public List<DbSchemaColumn> Columns { get { return columns; } }

        public string table_catalog//Catalog of the table.
        {
            get { return items["TABLE_CATALOG"]; }
        }

        public string table_schema//Schema that contains the table.
        {
            get { return items["TABLE_SCHEMA"];}
        }

        public string table_name//Table name.
        {
            get { return items["TABLE_NAME"];}
        }

        public string table_type//Type of table.Can be VIEW or BASE TABLE.
        {
            get { return items["TABLE_TYPE"]; }
        }
    }

    class DbSchemaColumn
    {
        Dictionary<string, string> items = new Dictionary<string, string>();
        public void SetItem(string name, string value)
        {
            if (items.ContainsKey(name)) items[name] = value;
            else items.Add(name, value);
        }

        public string table_catalog//Catalog of the table.
        {
            get { return items["TABLE_CATALOG"]; }
        }

        public string table_schema//Schema that contains the table.
        {
            get { return items["TABLE_SCHEMA"]; }
        }

        public string table_name//Table name.
        {
            get { return items["TABLE_NAME"];}
        }

        public string column_name//Column name.
        {
            get { return items["COLUMN_NAME"];}
        }

        public int ordinal_position//Column identification number.
        {
            get { return int.Parse(items["ORDINAL_POSITION"]); }
        }

        public string column_default//Default value of the column
        {
            get { return items["COLUMN_DEFAULT"]; }
        }

        public string is_nullable//Nullability of the column.If this column allows NULL, this column returns YES.Otherwise, No is returned.
        {
            get { return items["IS_NULLABLE"]; }
        }

        public string data_type//System-supplied data type.
        {
            get { return items["DATA_TYPE"]; }
        }

        public int character_maximum_length//Maximum length, in characters, for binary data, character data, or text and image data.Otherwise, NULL is returned.
        {
            get { return int.Parse(items["CHARACTER_MAXIMUM_LENGTH"]); }
        }

        public int character_octet_length//Maximum length, in bytes, for binary data, character data, or text and image data.Otherwise, NULL is returned.
        {
            get { return int.Parse(items["CHARACTER_OCTET_LENGTH"]); }
        }

        public int numeric_precision//Unsigned Byte Precision of approximate numeric data, exact numeric data, integer data, or monetary data.Otherwise, NULL is returned.
        {
            get { return int.Parse(items["NUMERIC_PERCISION"]); }
        }

        public int numeric_precision_radix//Int16 Precision radix of approximate numeric data, exact numeric data, integer data, or monetary data.Otherwise, NULL is returned.
        {
            get { return int.Parse(items["NUMERIC_PERCISION_RADIX"]); }
        }

        public int numeric_scale//Int32 Scale of approximate numeric data, exact numeric data, integer data, or monetary data.Otherwise, NULL is returned.
        {
            get { return int.Parse(items["NUMERIC_SCALE"]); }
        }

        public int datetime_precision//Int16 Subtype code for datetime and SQL-92 interval data types.For other data types, NULL is returned.
        {
            get { return int.Parse(items["DATETIME_PERCISION"]); }
        }

        public string character_set_catalog//Returns master, indicating the database in which the character set is located, if the column is character data or text data type. Otherwise, NULL is returned.
        {
            get { return items["CHARACTER_SET_CATALOG"]; }
        }

        public string character_set_schema//Always returns NULL.
        {
            get { return items["CHARACTER_SET_SCHEMA"]; }
        }

        public string character_set_name//Returns the unique name for the character set if this column is character data or text data type. Otherwise, NULL is returned.
        {
            get { return items["CHARACTER_SET_NAME"]; }
        }

        public string collation_catalog//Returns master, indicating the database in which the collation is defined, if the column is character data or text data type. Otherwise, this column is NULL.
        {
            get { return items["COLLATION_CATALOG"]; }
        }

        public int item_int(string name)
        {
            return int.Parse(items[name]);
        }

        public string item_string(string name)
        {
            return items[name];
        }

        public string TableCatalog;//0 TABLE_CATALOG

        public string TableSchema;//1 TABLE_SCHEMA
        public string TableName;//2 TABLE_NAME
        public string CloumeName;//3 COLUMN_NAME
        public int OrdinalPosition;//4 ORDINAL_POSITION
        public object ColumnDefault;//5 COLUMN_DEFAULT
        public bool IsNullable;//6 IS_NULLABLE
        public string DataType;//7 DATA_TYPE
        public int CharacterMaximumLength;//8 CHARACTER_MAXIMUM_LENGTH
        public int NumericPrecision;//9 NUMERIC_PRECISION
        public int NumericScale;//10 NUMERIC_SCALE
        public int DatetimePrecision;//11 DATETIME_PRECISION
        public string CharacterSetName;//12 CHARACTER_SET_NAME
        public string CollationName;//13 COLLATION_NAME
        public string ColumdType;//14 COLUMN_TYPE
        public bool ColumnKey;// string 15 COLUMN_KEY
        public string Extra;//16 EXTRA
        public string Privileges;//17 PRIVILEGES
        public string ColumnComment;//18 COLUMN_COMMENT
        public string GenerationExpression;//19 GENERATION_EXPRESSION
    }

    class DbModel
    {
        public string ClassName;
        public Dictionary<string, string> DataMembers = new Dictionary<string, string>();

        public DbModel(string name=null)
        {
            ClassName = name;
        }

        public override string ToString()
        {
            string str = "class " + ClassName + "\n{\n";
            foreach(string key in DataMembers.Keys)
            {
                str += "\tpublic " + DataMembers[key] + "\t" + key + ";\n";
            }
            str += "}\n";
            return str;
        }
    }

    class Db2Sharp
    {
        protected string ConnectionString = null;
        public List<DbSchemaTable> Tables =new List<DbSchemaTable>();
        protected DbConnection dbConnection = null;

        protected Dictionary<string, string> DbType2SharpType = new Dictionary<string, string>();

        public virtual bool OpenConnection()
        {
            try
            {
                if (dbConnection == null) return false;
                if (dbConnection.State == ConnectionState.Open)
                {
                    return true;
                }
                dbConnection.Open();
                return true;
            }
            catch(DbException ex)
            {
                Show(ex);    
            }
            return false;
        }

        public void CloseConnection()
        {
            if (dbConnection.State == ConnectionState.Open)
            {
                dbConnection.Close();
            }
        }

        public void Show(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        public void Show(string msg)
        {
            Console.Write(msg);
        }

        public void ShowLine(string msg)
        {
            Console.WriteLine(msg);
        }

        public void GetSchema()
        {
            GetTables();
            foreach (DbSchemaTable table in Tables)
            {
                ShowLine(table.table_name);
                GetTableColumns(table);
            }
        }

        void GetTables()
        {
            try
            {
                Tables.Clear();
                OpenConnection();
                DataTable schema = dbConnection.GetSchema("Tables");
                foreach(DataRow row in schema.Rows)
                {
                    DbSchemaTable table = new DbSchemaTable();
                    foreach(DataColumn col in schema.Columns)
                    {
                        table.SetItem(col.ColumnName, row[col.ColumnName].ToString());
                    }
                    Tables.Add(table);
                }
            }
            catch(DbException ex)
            {
                Show(ex);
            }
            CloseConnection();
        }

        void GetTableColumns(DbSchemaTable table)
        {
            try
            {
                OpenConnection();
                string table_name = table.table_name;
                DataTable schema = dbConnection.GetSchema("Columns");
                foreach (DataRow row in schema.Rows)
                {
                    if (row["TABLE_NAME"].ToString() == table_name)
                    {
                        DbSchemaColumn column = new DbSchemaColumn();
                        foreach (DataColumn col in schema.Columns)
                        {
                            column.SetItem(col.ColumnName, row[col.ColumnName].ToString());
                        }
                        table.AddColumn(column);
                    }
                }
            }
            catch (DbException ex)
            {
                Show(ex);
            }
            CloseConnection();
        }

        public virtual void   Init()
        {

        }

        public string TableName2ModelName(string tablename)
        {
            string ModelName = "";
            bool first_char = true;
            foreach (char ch in tablename)
            {
                if (ch == '_')
                {
                    first_char = true;
                    continue;
                }
                if (first_char)
                {
                    ModelName += char.ToUpper(ch);
                    first_char = false;
                }
                else ModelName += ch;
            }
            return ModelName;
        }

        public virtual string GetSharpTypeName(string DbTypeName)
        {
            return DbTypeName;
        }

        public DbModel CreateModule(DbSchemaTable table)
        {
            string ModelName = TableName2ModelName(table.table_name);
            DbModel model = new DbModel(ModelName);
            foreach(DbSchemaColumn column in table.Columns)
            {
                string colname = column.column_name;
                string typename = GetSharpTypeName(column.data_type);
                model.DataMembers[colname] = typename;
            }
            return model;
        }

    }
}
