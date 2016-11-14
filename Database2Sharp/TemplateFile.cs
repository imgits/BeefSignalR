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
    partial class User
    {
        public int? id { get; set; }
        public string username { get; set; }
        public string password { get ; set ; }
        public DateTime? create_time { get; set; }
        public byte[] info { get; set; }
    }

    partial class TestTemplateClass
    {
        string ConnectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=test;Integrated Security=True";
        Dictionary<string, string> select_colums_list = new Dictionary<string, string>();

        void Log(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        async Task<int> mysql_get_last_insert_id(SqlConnection conn)
        {
            SqlCommand cmd = new SqlCommand("SELECT LAST_INSERT_ID()", conn);
            int last_id = (int)(await cmd.ExecuteScalarAsync());
            return last_id;
        }

        //ms sql
        async Task<int> get_last_insert_id<T>(SqlConnection conn) where T : class
        {
            string sql = string.Format(@"SELECT IDENT_CURRENT('{0}')", typeof(T).Name);
            SqlCommand cmd = new SqlCommand(sql,conn);
            object result = await cmd.ExecuteScalarAsync();
            if (result == null) return -1;
            int last_id = Convert.ToInt32(result);
            return last_id;
        }

        bool SetPropertyValue<T>(T t, string name, object value) where T : class
        {
            try
            {
                PropertyInfo pi = typeof(T).GetProperty(name);
                if (pi == null) return false;
                pi.SetValue(t, value);
            }
            catch(Exception ex)
            {
                Log(ex);
            }
            return false;
        }

        object GetPropertyValue<T>(T t, string name) where T : class
        {
            try
            {
                PropertyInfo pi = typeof(T).GetProperty(name);
                if (pi == null) return null;
                return pi.GetValue(t);
            }
            catch (Exception ex)
            {
                Log(ex);
            }
            return false;
        }

        /// <summary>
        /// 添加单个实体对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        async public Task<T> add<T>(T t) where T : class, new()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    SqlCommand cmd = new SqlCommand(null, conn);
                    set_insert_params(cmd, t);
                    int ret = await cmd.ExecuteNonQueryAsync();//.ExecuteNonQuery();
                    if (ret != 1) return null;
                    int last_insert_id = await get_last_insert_id<T>(conn);
                    if (last_insert_id <= 0) return null;
                    SetPropertyValue(t,"id",last_insert_id);
                    return t;
                }
            }
            catch (SqlException ex)
            {
                Log(ex);
            }
            return null;
        }

        async public Task<T> get<T>(int id) where T : class, new()
        {
            string where_define ="id=" + id;
            List<T> tlist = await get<T>(where_define);
            if (tlist != null && tlist.Count > 0) return tlist[0];
            return null;
        }

        async public Task<List<T>> get<T>(string where_define) where T : class, new()
        {
            try
            {
                List<T> tlist = new List<T>();
                string sql = string.Format("SELECT * FROM [{0}] WHERE {1}", typeof(T).Name, where_define);//select_sql<T>(where_define);
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    
                    //将数据库字段与类属性顺序对应起来
                    PropertyInfo[] pi = new PropertyInfo[reader.FieldCount];
                    Type type = typeof(T);
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string field_name = reader.GetName(i);
                        pi[i] = type.GetProperty(field_name);
                    }

                    while (await reader.ReadAsync())
                    {
                        T t = select_read<T>(reader,pi);
                        tlist.Add(t);
                    }
                    return tlist;
                }
            }
            catch (SqlException ex)
            {
                Log(ex);
            }
            return null;
        }

        public List<dynamic> get<T>(string columns, string where_define) where T : class, new()
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
            catch (SqlException ex)
            {
                Log(ex);
            }
            return null;
        }

        async public Task<bool> set<T>(T t) where T : class, new()
        {
            int id = (int)GetPropertyValue(t,"id");
            string where_define = "id=" + id;
            return await set<T>(t, where_define);
        }

        async public Task<bool> set<T>(T t, string where_define) where T : class
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    SqlCommand cmd = new SqlCommand(null, conn);
                    set_update_params<T>(cmd, t, where_define);
                    int result = await cmd.ExecuteNonQueryAsync();
                    return true;
                }
            }
            catch (SqlException ex)
            {
                Log(ex);
            }
            return false;
        }

        async public Task<bool> del<T>(int id) where T : class, new()
        {
            string where_define = "id=" + id;
            return await del<T>(where_define);
        }

        async public Task<bool> del<T>(string where_define) where T : class, new()
        {
            try
            {
                T t = new T();
                string sql = string.Format("DELETE FROM [{0}] WHERE {1}",typeof(T).GetType().Name,where_define);
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    int result = await cmd.ExecuteNonQueryAsync();
                    return true;
                }
            }
            catch (SqlException ex)
            {
                Log(ex);
            }
            return false;
        }

        void   set_insert_params<T>(SqlCommand cmd,T t) where T : class
        {
            Type type = typeof(T);
            //PropertyInfo[] properties = type.GetProperties(BindingFlags.Public);
            PropertyInfo[] properties = type.GetProperties();
            string column_names = "";
            string column_params = "";
            char comma = ' ';
            foreach (PropertyInfo property in properties)
            {
                if (property.Name == "id") continue;
                object value = property.GetValue(t);
                if (value == null) continue;
                column_names += comma + property.Name;
                column_params += comma + "@" + property.Name;
                comma = ',';
                cmd.Parameters.AddWithValue('@' + property.Name, value);
            }
            string sql = string.Format("INSERT INTO [{0}] ({1}) VALUES({2})", type.Name, column_names, column_params);
            cmd.CommandText = sql;
        }

        void   set_update_params<T>(SqlCommand cmd, T t, string where_define) where T : class
        {
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();
            string column_name_params = "";
            char comma = ' ';
            foreach (PropertyInfo property in properties)
            {
                if (property.Name == "id") continue;
                object value = property.GetValue(t);
                if (value == null) continue;
                column_name_params += comma + property.Name +"=@" + property.Name;
                comma = ',';
                cmd.Parameters.AddWithValue('@' + property.Name, value);
            }
            string sql = string.Format("UPDATE [{0}] SET {1} WHERE {2}", type.Name.ToLower(), column_name_params, where_define);
            cmd.CommandText = sql;
        }

        T select_read<T>(DbDataReader reader, PropertyInfo[] pi) where T : class, new()
        {
            T t = new T();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                object field_value = reader.GetValue(i);
                pi[i].SetValue(t, field_value);
            }
            return t;
        }
    }

    class Model
    {

    }

    class TemplateClass
    {
        string ConnectionString = "";

        async private Task<int> get_last_insert_id(SqlConnection conn)
        {
            string sql = "SELECT LAST_INSERT_ID()";
            SqlCommand cmd = new SqlCommand(sql, conn);
            int result = (int)await cmd.ExecuteScalarAsync();
            return result;
        }

        void Show(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        //InsertStatementStart
        async public Task<Model> add_model(Model model)
        {
            try
            {
                string sql = "INSERT INTO [model] (ColunmNameList) VALUES (ColumnParamList)";
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    //SetParameters
                    int ret = await cmd.ExecuteNonQueryAsync();
                    int last_insert_id = await get_last_insert_id(conn);
                    model = await get_model(last_insert_id);
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
        async public Task<Model> get_model(int id)
        {
            try
            {
                string sql = string.Format("SELECT * FROM table_name WHERE id={0}", id);
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
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
        async public Task<List<Model>> get_models(string where_definition)
        {
            try
            {
                List<Model> models = new List<Model>();
                string sql = string.Format("SELECT * FROM table_name WHERE {0}", where_definition);
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
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
        async public Task<bool> set_model(Model model)
        {
            try
            {
                string sql = "UPDATE table_name SET NameValueList WHERE id=@id";
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    //SetParameters
                    int ret = await cmd.ExecuteNonQueryAsync();
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
        async public Task<bool> del_model(int id)
        {
            try
            {
                string sql = string.Format("DELETE FROM table_name WHERE id={0}", id);
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    int ret = await cmd.ExecuteNonQueryAsync();
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
        async public Task<bool> del_model(string where_definition)
        {
            try
            {
                string sql = string.Format("DELETE FROM table_name WHERE {0}", where_definition);
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    int ret = await cmd.ExecuteNonQueryAsync();
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

}
