using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Data.SqlClient;

namespace Database2Sharp
{
    class dbmodel
    {
        public virtual string sql_add() { return null; }
    }

    class User
    {
        public int id;
        public string username;
        public string password;
        public User() { }

        public User(SqlDataReader reader)
        {
            this.id = reader.GetInt32(0);
            this.username = reader.GetString(1);
            this.password = reader.GetString(2);
        }

        public static string sql_add()
        {
            return "INSERT INTO user (username,password) VALUES(@username,@password)";
        }

        public void sql_add_params(DbCommand cmd)
        {
            cmd.Parameters.Insert(0,this.username);
            cmd.Parameters.Insert(1, this.username);
        }

        public static string sql_get(int id)
        {
            return "SELECT * FROM user while id=" +id;
        }

        public static string sql_gets(string where_define)
        {
            return "SELECT * FROM user while " + where_define;
        }

        public static string sql_set(int id)
        {
            return "UPDATE user SET username=@username, password=@password WHERE id=" + id;
        }

        public void sql_set_params(DbCommand cmd)
        {
            cmd.Parameters.Insert(0, this.username);
            cmd.Parameters.Insert(1, this.username);
        }

        public static string sql_del(int id)
        {
            return "DELETE FROM user while id=" + id;
        }

        public static string sql_dels(string where_define)
        {
            return "DELETE FROM user while "+ where_define;
        }

    }

    class Model
    {

    }


    class TemplateFile
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

        public User    add(User user)
        {
            try
            {
                string sql = User.sql_add();
                using (SqlConnection conn = new SqlConnection())
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    user.sql_add_params(cmd);
                    int ret = cmd.ExecuteNonQuery();
                    int last_insert_id = get_last_insert_id(conn);
                    user = get_user(last_insert_id);
                    return user;
                }
            }
            catch (MySqlException ex)
            {
            }
            return null;
        }
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
                        user.password = reader.GetString("password");
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
                        user.password = reader.GetString("password");
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
                        user.password = reader.GetString("password");
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
