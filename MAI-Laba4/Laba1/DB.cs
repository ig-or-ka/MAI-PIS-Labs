using MySqlConnector;
using System.Collections.Generic;
using System.IO;

namespace Laba1
{
    public class User
    {
        public bool Selected { get; set; }
        public int ID { get => _ID; }
        public string Name { get => _Name; set { _Name = value; modify = true; } }
        public Posts Post { get => (Posts)_Post; set { _Post = (int)value; modify = true; } }
        public string Address { get => _Address; set { _Address = value; modify = true; } }
        public string Phone { get => _Phone; set { _Phone = value; modify = true; } }
        public string Password { get => _Password; set { _Password = value; modify = true; } }

        int _ID = -1;
        string _Name = "";
        string _Password = "";
        int _Post = 0;
        string _Address = "";
        string _Phone = "";
        bool modify = false;
        DB connection;

        public User(int ID, MySqlDataReader reader, DB connection)
        {
            _ID = ID;
            _Name = reader.GetString(1);
            _Post = reader.GetInt32(2);
            _Address = reader.GetString(3);
            _Phone = reader.GetString(4);
            _Password = reader.GetString(5);
            this.connection = connection;
        }

        public User(DB connection)
        {
            this.connection = connection;
        }

        public bool Save()
        {
            if(_ID == -1)
            {
                connection.Execute($"insert into Users (name, post, address, phone, password) values ('{_Name}', '{_Post}', '{_Address}', '{_Phone}', '{_Password}')");

                var reader = connection.ExecuteReader($"select id from Users where name='{_Name}' and post='{_Post}' and address='{_Address}' and phone='{_Phone}' and password='{_Password}'");

                if(reader.Read())
                {
                    _ID = reader.GetInt32(0);
                    connection.users[_ID] = this;
                }
                else
                {
                    //TODO
                }

                reader.Close();
                modify = false;

                return true;
            }
            else if(modify)
            {
                //Console.WriteLine($"Change: {_ID}");
                connection.Execute($"update Users set name='{_Name}', post='{_Post}', address='{_Address}', phone='{_Phone}', password='{_Password}' where id = {_ID}");
                modify = false;

                return true;
            }

            return false;
        }
    }
    public class DB
    {
        static string connect_line = $"Server=localhost; database=laba; UID=root; password={File.ReadAllText("dbpass.txt")}";
        MySqlConnection connection = new MySqlConnection(connect_line);

        public Dictionary<int, User> users = new Dictionary<int, User>();
        List<User> users_list = new List<User>();

        public DB()
        {
            connection.Open();
            ReadUsers();
        }

        public User[] Users
        {
            get
            {
                var res = new User[users_list.Count];
                users_list.CopyTo(res);
                return res;
            }
        }

        public User NewUser()
        {
            var usr = new User(this);
            users_list.Add(usr);
            return usr;
        }

        public void Update()
        {
            users.Clear();
            users_list.Clear();
            ReadUsers();
        }

        public void Remove(User user)
        {
            if(user.ID != -1)
            {
                Execute($"delete from Users where id = {user.ID}");
                users.Remove(user.ID);
            }
            users_list.Remove(user);
        }

        void ReadUsers()
        {
            var reader = ExecuteReader($"select * from Users");

            while (reader.Read())
            {
                var ID = reader.GetInt32(0);
                var usr = new User(ID, reader, this);
                users[ID] = usr;
                users_list.Add(usr);
            }

            reader.Close();
        }

        public User? AuthUser(string name, string password)
        {
            var reader = ExecuteReader($"select * from Users where name = '{name}' and password = '{password}'");

            User? usr = null;

            if (reader.Read())
            {
                var ID = reader.GetInt32(0);

                if (users.ContainsKey(ID))
                {
                    usr = users[ID];
                }
                else
                {
                    usr = new User(ID, reader, this);
                    users[ID] = usr;
                    users_list.Add(usr);
                }                
            }
            reader.Close();

            return usr;
        }

        public void Execute(string sql)
        {
            var cmd = new MySqlCommand(sql, connection);
            cmd.ExecuteNonQuery();
        }

        public MySqlDataReader ExecuteReader(string sql)
        {
            var cmd = new MySqlCommand(sql, connection);
            return cmd.ExecuteReader();
        }

        public void Close() => connection.Close();
    }
}
