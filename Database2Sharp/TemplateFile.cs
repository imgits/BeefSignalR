using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Data.SqlClient;
using System.Dynamic;
using System.Reflection;

namespace Database2Sharp
{
    //EntityBase_Start
    class EntityBase
    {
        public int id { get; set; }

        public virtual void Init(DbDataReader reader) { }
        public virtual string insert_sql() { return null; }
        public virtual string update_sql(int id) { return null; }
        public virtual string update_sql(string where_define) { return null; }

        public virtual string select_sql(int id)
        {
            string sql = string.Format("SELECT * FROM {0} WHRER id={1}", this.GetType().Name, id);
            return sql;
        }

        public virtual string select_sql(string where_define)
        {
            string sql = string.Format("SELECT * FROM {0} WHRER ", this.GetType().Name, where_define);
            return sql;
        }

        public string delete_sql(int id)
        {
            string sql = string.Format("DELETE FROM {0} WHRER id={1}",this.GetType().Name,id);
            return sql;
        }

        public string delete_sql(string where_define)
        {
            string sql = string.Format("DELETE FROM {0} WHRER ", this.GetType().Name, where_define);
            return sql;
        }

        public virtual void set_sql_params(DbCommand cmd) { }
    }
    //EntityBase_End

    partial class User : EntityBase
    {
        //public int id { get; set; }

        public string username { get; set; }
        public byte[] password { get ; set ; }
        public DateTime create_time { get; set; }
        public string info { get; set; }

        public override void Init(DbDataReader reader)
        {
            this.id = reader.GetInt32(0);
            this.username = reader.GetString(1);
            //this.password = reader.GetString(2);
        }

        public override string insert_sql()
        {
            return "INSERT INTO user (username,password) VALUES(@username,@password)";
        }

        public override void set_sql_params(DbCommand cmd)
        {
            cmd.Parameters.Insert(0,this.username);
            cmd.Parameters.Insert(1, this.password);
        }

        public override string update_sql(int id)
        {
            return "UPDATE user SET username=@username, password=@password WHERE id=" + id;
        }

        public override string update_sql(string where_define)
        {
            return "UPDATE user SET username=@username, password=@password WHERE " + where_define;
        }

    }

    class Model
    {

    }

    class TemplateClass
    {
        string ConnectionString = "";

        private int get_last_insert_id(SqlConnection conn)
        {
            string sql = "SELECT LAST_INSERT_ID()";
            SqlCommand cmd = new SqlCommand(sql, conn);
            int result = (int)cmd.ExecuteScalar();
            return result;
        }

        void Show(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        //InsertStatementStart
        public Model add_model(Model model)
        {
            try
            {
                string sql = "INSERT INTO table_name (ColunmNameList) VALUES (ColumnParamList)";
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    //SetParameters
                    int ret = cmd.ExecuteNonQuery();
                    int last_insert_id = get_last_insert_id(conn);
                    model = get_model(last_insert_id);
                    return model;
                }
            }
            catch (SqlException ex)
            {
                Show(ex);
            }
            return null;
        }
        //InsertStatementEnd

        //SelectStatementStart
        public Model get_model(int id)
        {
            try
            {
                string sql = string.Format("SELECT * FROM table_name WHERE id={0}", id);
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    //DefineStream
                    if (reader.Read())
                    {
                        Model model = new Model();
                        //ReadColumns
                        return model;
                    }
                }
            }
            catch (SqlException ex)
            {
                Show(ex);
            }
            return null;
        }
        //SelectStatementEnd

        //SelectStatement1Start
        public List<Model> get_models(string where_definition)
        {
            try
            {
                List<Model> models = new List<Model>();
                string sql = string.Format("SELECT * FROM table_name WHERE {0}", where_definition);
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    //DefineStream
                    while (reader.Read())
                    {
                        Model model = new Model();
                        //ReadColumns
                        models.Add(model);
                    }
                    return models;
                }
            }
            catch (SqlException ex)
            {
                Show(ex);
            }
            return null;
        }
        //SelectStatement1End

        //UpdateStatementStart
        public bool set_model(Model model)
        {
            try
            {
                string sql = "UPDATE table_name SET NameValueList WHERE id=@id";
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    //SetParameters
                    int ret = cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (SqlException ex)
            {
            }
            return false;
        }
        //UpdateStatementEnd

        //DeleteStatementStart
        public bool del_model(int id)
        {
            try
            {
                string sql = string.Format("DELETE FROM table_name WHERE id={0}", id);
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    int ret = cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (SqlException ex)
            {
                Show(ex);
            }
            return false;
        }
        //DeleteStatementEnd

        //DeleteStatement1Start
        public bool del_model(string where_definition)
        {
            try
            {
                string sql = string.Format("DELETE FROM table_name WHERE {0}", where_definition);
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    int ret = cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (SqlException ex)
            {
                Show(ex);
            }
            return false;
        }
        //DeleteStatement1End
    }

    class TestTemplateClass
    {
        string ConnectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=test;Integrated Security=True";
        //CURD_Start
        int get_last_insert_id(SqlConnection conn)
        {
            SqlCommand cmd = new SqlCommand("SELECT LAST_INSERT_ID()", conn);
            int last_id = (int)cmd.ExecuteScalar();
            return last_id;
        }

        public T add<T>(T t) where T : class, new()
        {
            try
            {
                string sql = insert_sql(t);
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    int ret = cmd.ExecuteNonQuery();
                    int last_insert_id = get_last_insert_id(conn);
                    t = get<T>(last_insert_id);
                    return t;
                }
            }
            catch (MySqlException ex)
            {
            }
            return null;
        }

        public T get<T>(int id) where T : class, new()
        {
            try
            {
                string sql = select_sql<T>("id=" + id);
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        T t = select_read<T>(reader);
                        return t;
                    }
                }
            }
            catch (MySqlException ex)
            {
            }
            return null;
        }

        public List<T> gets<T>(string where_define) where T : class, new()
        {
            try
            {
                List<T> tlist = new List<T>();
                string sql = select_sql<T>(where_define);
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        T t = select_read<T>(reader);
                        tlist.Add(t);
                    }
                    return tlist;
                }
            }
            catch (MySqlException ex)
            {
            }
            return null;
        }

        public List<dynamic> gets<T>(string columns, string where_define) where T : EntityBase, new()
        {
            try
            {
                List<dynamic> tlist = new List<dynamic>();
                string[] colnames = columns.Split(new char[] { ',' });
                string sql = string.Format("SELECT {0} FROM {1} WHERE {2}", columns, typeof(T).Name, where_define);
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        dynamic dt = new ExpandoObject();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            dt[colnames[i]] = reader.GetValue(i);
                        }
                        tlist.Add(dt);
                    }
                    return tlist;
                }
            }
            catch (MySqlException ex)
            {
            }
            return null;
        }

        public bool set<T>(T t) where T : class, new()
        {
            try
            {
                PropertyInfo pi = t.GetType().GetProperty("id");
                int id = (int)pi.GetValue(t);
                string sql = update_sql(t, "id=" + id);
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    int result = cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (MySqlException ex)
            {
            }
            return false;
        }

        public bool sets<T>(dynamic t, string where_define) where T : class
        {
            try
            {
                string set_values = "";
                int count = 0;
                foreach (var property in (IDictionary<String, Object>)t)
                {
                    if (count++ > 0) set_values += ',';
                    set_values += property.Key + "='" + property.Value + "'";
                }
                string sql = string.Format("UPDATE {0} SET {1} WHERE {2}", typeof(T).Name, set_values, where_define);

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    int result = cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (MySqlException ex)
            {
            }
            return false;
        }

        public bool del<T>(int id) where T : class, new()
        {
            string where_define = "id=" + id;
            return del<T>(where_define);
        }

        public bool del<T>(string where_define) where T : class, new()
        {
            try
            {
                T t = new T();
                string sql = string.Format("DELETE FROM {0} WHERE {1}",typeof(T).GetType().Name,where_define);
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    int result = cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (MySqlException ex)
            {
            }
            return false;
        }

        string insert_sql<T>(T t) where T : class
        {
            Type type = typeof(T);
            //PropertyInfo[] properties = type.GetProperties(BindingFlags.Public);
            PropertyInfo[] properties = type.GetProperties();
            string column_names = "";
            string column_values = "";
            char comma = ' ';
            foreach (PropertyInfo property in properties)
            {
                if (property.Name == "id") continue;
                column_names += comma + property.Name;
                object value = property.GetValue(t);
                column_values += comma;
                column_values += (value == null) ? "\'\'" : '\'' + value.ToString() + '\'';
                comma = ',';
            }
            string sql = string.Format("INSERT INTO {0} ({1}) VALUES({2})",  type.Name.ToLower(), column_names, column_values);
            return sql;
        }

        string update_sql<T>(T t, string where_define) where T : class
        {
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();// ((BindingFlags.Instance | BindingFlags.Public);
            string column_name_values = "";
            char comma = ' ';
            foreach (PropertyInfo property in properties)
            {
                if (property.Name == "id") continue;
                column_name_values += comma + property.Name + "='" + property.GetValue(t).ToString() + '\'';
                comma = ',';
            }
            string sql = string.Format("UPDATE {0} SET ({1}) WHERE {2}", type.Name, column_name_values, where_define);
            return sql;
        }

        string select_sql<T>(string where_define)
        {
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();// ((BindingFlags.Instance | BindingFlags.Public);
            string column_names = "";
            char comma = ' ';
            foreach (PropertyInfo property in properties)
            {
                if (property.Name == "id") continue;
                column_names += comma + property.Name;
                comma = ',';
            }
            string sql = string.Format("SELECT {0} FROM ({1}) WHERE {2}", column_names, type.Name, where_define);
            return sql;
        }

        T select_read<T>(DbDataReader reader) where T : class, new()
        {
            T t = new T();
            Type type = typeof(T);
            for (int i = 0; i < reader.FieldCount; i++)
            {
                string filed_name = reader.GetName(i);
                PropertyInfo pi = type.GetProperty(filed_name);
                pi.SetValue(t, reader.GetValue(i));
            }
            return t;
        }
    }


    class TestTemplateClass1
    {
        //CURD_Start
        public T add<T>(T t) where T : EntityBase
        {
            try
            {
                string sql = t.insert_sql();
                using (SqlConnection conn = new SqlConnection())
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    t.set_sql_params(cmd);
                    int ret = cmd.ExecuteNonQuery();
                    //int last_insert_id = get_last_insert_id(conn);
                    //t = get_user(last_insert_id);
                    return t;
                }
            }
            catch (MySqlException ex)
            {
            }
            return null;
        }

        public T get<T>(int id) where T : EntityBase, new()
        {
            try
            {
                T t = new T();
                string sql = t.select_sql(id);
                using (SqlConnection conn = new SqlConnection())
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        t.Init(reader);
                    }
                    return t;
                }
            }
            catch (MySqlException ex)
            {
            }
            return null;
        }

        public List<T> gets<T>( string where_define) where T : EntityBase, new()
        {
            try
            {
                List<T> tlist = new List<T>();
                T t = new T();
                string sql = t.select_sql(where_define);
                using (SqlConnection conn = new SqlConnection())
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        T tt = new T();
                        tt.Init(reader);
                        tlist.Add(tt);
                    }
                    return tlist;
                }
            }
            catch (MySqlException ex)
            {
            }
            return null;
        }

        public List<dynamic> gets<T>(string columns, string where_define) where T : EntityBase, new()
        {
            try
            {
                List<dynamic> tlist = new List<dynamic>();
                string[] colnames = columns.Split(new char[] { ',' });
                string sql = string.Format("SELECT {0} FROM {1} WHERE {2}", columns, typeof(T).Name, where_define);
                using (SqlConnection conn = new SqlConnection())
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        dynamic dt = new ExpandoObject();
                        for(int i =0; i < reader.FieldCount;i++)
                        {
                            dt[colnames[i]] = reader.GetValue(i);
                        }
                        tlist.Add(dt);
                    }
                    return tlist;
                }
            }
            catch (MySqlException ex)
            {
            }
            return null;
        }

        public bool set<T>(T t) where T : EntityBase, new()
        {
            try
            {
                string sql = t.update_sql(t.id);
                using (SqlConnection conn = new SqlConnection())
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    t.set_sql_params(cmd);
                    int result = cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (MySqlException ex)
            {
            }
            return false;
        }

        public bool sets<T>(dynamic t, string where_define) where T: class
        {
            try
            {
                string set_values = "";
                int count = 0;
                foreach (var property in (IDictionary<String, Object>)t)
                {
                    //if (property.Key == "where") continue;
                    if (count++ > 0) set_values += ',';
                    set_values += property.Key + "='" + property.Value + "'";
                }
                string sql = string.Format("UPDATE {0} SET {1} WHERE {2}",typeof(T).Name, set_values, where_define);
                
                using (SqlConnection conn = new SqlConnection())
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    int result = cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (MySqlException ex)
            {
            }
            return false;
        }

        public bool del<T>(int id) where T : EntityBase, new()
        {
            try
            {
                T t = new T();
                string sql = t.delete_sql(id);
                using (SqlConnection conn = new SqlConnection())
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    int result = cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (MySqlException ex)
            {
            }
            return false;
        }

        public bool dels<T>(string where_define) where T : EntityBase, new()
        {
            try
            {
                T t = new T();
                string sql = t.delete_sql(where_define);
                using (SqlConnection conn = new SqlConnection())
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    int result = cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (MySqlException ex)
            {
            }
            return false;
        }

        string insert_sql<T>(T t) where T : class
        {
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();// ((BindingFlags.Instance | BindingFlags.Public);
            string column_names = "";
            string column_values = "";
            char comma = ' ';
            foreach (PropertyInfo property in properties)
            {
                if (property.Name == "id") continue;
                column_names += comma + property.Name;
                column_values += '\'' + property.GetValue(t).ToString() + '\'';
                comma = ',';
            }
            string sql = string.Format("INSERT INTO {0} ({1}) VALUES({2})", column_names, type.Name , column_values);
            return sql;
        }

        string update_sql<T>(T t, string where_define) where T : class
        {
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();// ((BindingFlags.Instance | BindingFlags.Public);
            string column_name_values = "";
            char comma = ' ';
            foreach (PropertyInfo property in properties)
            {
                if (property.Name == "id") continue;
                column_name_values += comma + property.Name + "='" + property.GetValue(t).ToString() + '\'';
                comma = ',';
            }
            string sql = string.Format("UPDATE {0} SET ({1}) WHERE {2}", type.Name, column_name_values, where_define);
            return sql;
        }
        
        string select_sql<T>(string where_define)
        {
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();// ((BindingFlags.Instance | BindingFlags.Public);
            string column_names = "";
            char comma = ' ';
            foreach (PropertyInfo property in properties)
            {
                if (property.Name == "id") continue;
                column_names += comma + property.Name;
                comma = ',';
            }
            string sql = string.Format("SELECT {0} FROM ({1}) WHERE {2}", column_names, type.Name, where_define);
            return sql;
        }

        T select_read<T>(DbDataReader reader) where T : class, new()
        {
            T t = new T();
            Type type = typeof(T);
            for (int i =0; i < reader.FieldCount; i++)
            {
                string filed_name = reader.GetName(i);
                PropertyInfo pi = type.GetProperty(filed_name);
                pi.SetValue(t, reader.GetValue(i));
            }
            return t;
        }
    }

    class TestClass
    { 
        public User add_user(User user)  
        {
            try
            {
                string sql = "INSERT INTO user (username,password) VALUES (@username, @password)";
                using (MySqlConnection conn = new MySqlConnection())
                {
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@username", user.username);
                    cmd.Parameters.AddWithValue("@password", user.password);
                    int ret = cmd.ExecuteNonQuery();
                    //int last_insert_id = get_last_insert_id(conn);
                    //user = get_user(last_insert_id);
                    return user;
                }
            }
            catch(MySqlException ex)
            {
            }
            return null;
        }

        public bool set_user(User user)  //where  class t
        {
            try
            {
                string sql = "UPDATE user SET username=@username,password=@password WHERE id=@id";
                using (MySqlConnection conn = new MySqlConnection())
                {
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@id", user.id);
                    cmd.Parameters.AddWithValue("@username", user.username);
                    cmd.Parameters.AddWithValue("@password", user.password);
                    int ret = cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (MySqlException ex)
            {
            }
            return false;
        }

        public User get_user(int id)
        {
            try
            {
                string sql = string.Format("SELECT * FROM user WHERE id={0}",id);
                using (MySqlConnection conn = new MySqlConnection())
                {
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        User user = new User();
                        user.id = reader.GetInt32("id");
                        user.username = reader.GetString("username");
                        //user.password = reader.GetString("password");
                        return user;
                    }
                }
            }
            catch (MySqlException ex)
            {
            }
            return null;
        }

        public User get_user(string column_name, string column_value)
        {
            try
            {
                string sql = string.Format("SELECT * FROM user WHERE {0}='{1}'", column_name, column_value);
                using (MySqlConnection conn = new MySqlConnection())
                {
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        User user = new User();
                        user.id = reader.GetInt32("id");
                        user.username = reader.GetString("username");
                        //user.password = reader.GetString("password");
                        return user;
                    }
                }
            }
            catch (MySqlException ex)
            {
            }
            return null;
        }

        public User get_user(string where_definition)
        {
            try
            {
                string sql = string.Format("SELECT * FROM user WHERE ", where_definition);
                using (MySqlConnection conn = new MySqlConnection())
                {
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        User user = new User();
                        user.id = reader.GetInt32("id");
                        user.username = reader.GetString("username");
                        //user.password = reader.GetString("password");
                        return user;
                    }
                }
            }
            catch (MySqlException ex)
            {
            }
            return null;
        }

        public bool del_user(int id)
        {
            try
            {
                string sql = string.Format("DELETE FROM user WHERE id={0}", id);
                using (MySqlConnection conn = new MySqlConnection())
                {
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    int ret = cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (MySqlException ex)
            {
            }
            return false;
        }

        public bool del_user(string column_name, string column_value)
        {
            try
            {
                string sql = string.Format("DELETE FROM user WHERE {0}='{1}'", column_name, column_value);
                using (MySqlConnection conn = new MySqlConnection())
                {
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    int ret = cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (MySqlException ex)
            {
            }
            return false;
        }

        public bool del_user(string where_definition)
        {
            try
            {
                string sql = string.Format("DELETE FROM user WHERE {0}", where_definition);
                using (MySqlConnection conn = new MySqlConnection())
                {
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    int ret = cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (MySqlException ex)
            {
            }
            return false;
        }

    }
}
