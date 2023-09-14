using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using RPCServer.Server;
using System;
using System.Data;

namespace RPCServer.SQL
{
    public class SQLOperation
    {
        static string connectionString = "server=localhost;port=3306;User=root;Password=root;Database=gametable;pooling=false;CharSet=utf8";
        public static int CheckUser(string account, out int pid, string pwd = "")
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                if (connection.State == ConnectionState.Open)
                {
                    string sqlQuery;
                    if (string.IsNullOrEmpty(pwd))
                        sqlQuery = string.Format("SELECT * FROM users WHERE user = '{0}'", account);
                    else
                        sqlQuery = string.Format("SELECT * FROM users WHERE user = '{0}' AND pwd = '{1}'", account, pwd);
                    //sqlQuery = string.Format("SELECT COUNT(*) FROM flashusers WHERE user = '{0}' AND pwd = '{1}'", account, pwd);
                    MySqlCommand command = new MySqlCommand(sqlQuery, connection);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string name = reader["name"].ToString();
                            pid = reader.GetInt32(reader.GetOrdinal("id"));
                            Console.WriteLine($"查询角色[{name}]PID[{pid}]");
                            //返回-2则表示角色名为空
                            return string.IsNullOrEmpty(name) ? -2 : pid;
                        }
                    }
                    pid = -1;
                    return -1; // 找不到该角色
                }
                else
                {
                    pid = -1;
                    return -1; // 查询异常
                }
            }
        }
        public static int RegistUser(string account, string pwd)
        {
            int pid;
            if (CheckUser(account, out pid) != -1) return -1;//账号已存在
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string insertQuery = "INSERT INTO users (user,pwd,name) VALUES (@User,@Pwd,'')";
                MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection);
                insertCommand.Parameters.AddWithValue("@User", account);
                insertCommand.Parameters.AddWithValue("@Pwd", pwd);
                int rowsAffected = insertCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    // 插入成功，获取自增ID
                    MySqlCommand getIdCommand = new MySqlCommand("SELECT LAST_INSERT_ID()", connection);
                    int newUserId = Convert.ToInt32(getIdCommand.ExecuteScalar());
                    Console.WriteLine($"角色PID[{newUserId}]");
                    return newUserId;
                }
                return -1; // 插入失败，返回-1或其他适当的值
            }
        }
        public static int CreateHero(int pid, string info, string bag, string name)
        {
            //设置名称，判断是否有重名
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                if (connection.State == ConnectionState.Open)
                {
                    string sqlQuery = string.Format("SELECT COUNT(*) FROM users WHERE name = '{0}'", name);

                    MySqlCommand command = new MySqlCommand(sqlQuery, connection);

                    bool canCreate = Convert.ToInt32(command.ExecuteScalar()) != 1;

                    if (canCreate)
                    {
                        string updateQuery = "UPDATE users SET name=@Name where id=@ID";
                        MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection);
                        updateCommand.Parameters.AddWithValue("@ID", pid);
                        updateCommand.Parameters.AddWithValue("@Name", name);
                        int rowsAffected = updateCommand.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            sqlQuery = "SELECT COUNT(*) FROM infos WHERE pid =@PID";
                            command = new MySqlCommand(sqlQuery, connection);
                            command.Parameters.AddWithValue("@PID", pid);
                            string insertQuery;
                            if (Convert.ToInt32(command.ExecuteScalar()) >= 1)
                            {
                                //sqlQuery = "DELETE FROM infos WHERE pid =@PID";
                                //command = new MySqlCommand(sqlQuery, connection);
                                //command.Parameters.AddWithValue("@PID", pid);
                                //if (command.ExecuteNonQuery() <= 0)
                                //    return -1;//删除失败
                                insertQuery = "UPDATE infos SET info=@INFO , bag=@BAG where pid=@PID";
                            }
                            //这里还要检测是否存在PID的数据，如果存在需要先删除，或者改成UPDATE的
                            else insertQuery = "INSERT INTO infos (pid,info,bag) VALUES (@PID, @INFO,@BAG)";
                            MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection);
                            insertCommand.Parameters.AddWithValue("@PID", pid);
                            insertCommand.Parameters.AddWithValue("@INFO", info);
                            insertCommand.Parameters.AddWithValue("@BAG", bag);
                            rowsAffected = insertCommand.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                // 插入成功
                                return 0;
                            }
                            return -1; // 插入失败，返回-1或其他适当的值

                        }
                        return -1; // 更新失败，返回-1或其他适当的值
                    }
                    return -1;//创建异常
                }
                else return -1;//创建异常
            }
        }
        public static bool SaveJsonData(int pid, string which, string json)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                if (connection.State == ConnectionState.Open)
                {
                    string sqlQuery = string.Format("UPDATE infos SET {0}=@JSON WHERE pid = @PID", which);
                    MySqlCommand command = new MySqlCommand(sqlQuery, connection);
                    command.Parameters.AddWithValue("@JSON", json);
                    command.Parameters.AddWithValue("@PID", pid);
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
                return false;
            }
        }
        public static T GetJsonData<T>(int pid, string which)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                if (connection.State == ConnectionState.Open)
                {
                    string sqlQuery = string.Format("SELECT * FROM infos WHERE pid = '{0}'", pid);
                    MySqlCommand command = new MySqlCommand(sqlQuery, connection);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string data = reader[which].ToString();
                            return JsonConvert.DeserializeObject<T>(data);
                        }
                    }
                    return default; // 找不到该数据
                }
                else
                {
                    return default; // 查询异常
                }
            }
        }

        public static bool InsertChat(int PID, int UID, string msg)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                if (connection.State == ConnectionState.Open)
                {
                    DateTime curTime = DateTime.Now;
                    string insertQuery = "INSERT INTO chats (pid,tid,msg,time) VALUES (@PID,@TID,@Msg,@Time)";
                    MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection);
                    insertCommand.Parameters.AddWithValue("@PID", PID);
                    insertCommand.Parameters.AddWithValue("@TID", UID);
                    insertCommand.Parameters.AddWithValue("@Msg", msg);
                    insertCommand.Parameters.AddWithValue("@Time", curTime);
                    int rowsAffected = insertCommand.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
                return false;
            }
        }
        public static ChatMsgList GetChats(int pid, int tid)
        {
            ChatMsgList list = new ChatMsgList();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                if (connection.State == ConnectionState.Open)
                {
                    string Query = "SELECT * FROM chats where (pid=@PID and tid=@TID) or (pid=@TID and tid=@PID)";
                    MySqlCommand command = new MySqlCommand(Query, connection);
                    command.Parameters.AddWithValue("@PID", pid);
                    command.Parameters.AddWithValue("@TID", tid);
                    //insertCommand.ExecuteNonQuery();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ChatMsg data = new ChatMsg();
                            data.pid = tid;
                            data.type = ChatType.person;
                            int tpid = reader.GetInt32(reader.GetOrdinal("pid"));
                            data.name = GetJsonData<MyCharacter>(tpid, "info").name;
                            data.msg = reader["msg"].ToString();
                            list.list.Add(data);
                        }
                    }
                }
            }
            return list;
        }
    }
}
