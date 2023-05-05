using MySqlConnector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MAI_Laba7
{
    public class OSInfo
    {
        public string Code { get => _code; }
        public string Name { get => _os_name; }

        private string _os_name;
        private string _code;

        public OSInfo(string os_name, string code) {
            _os_name = os_name;
            _code = code;
        }
    }

    public class Division
    {
        public int division_id;
        DB db;
        public int count_os = 0;

        public Division(DB db, int id)
        {
            division_id = id;
            this.db = db;
        }

        public string CreateOS(string name)
        {
            count_os++;
            int os_id = count_os;

            string str_div_id = division_id.ToString();
            if (str_div_id.Length == 1)
            {
                str_div_id = $"0{str_div_id}";
            }

            string str_os_id = os_id.ToString();
            if (str_os_id.Length == 1)
            {
                str_os_id = $"0{str_os_id}";
            }

            var os_code = $"{str_div_id}{str_os_id}";

            var os_info = new OSInfo(name, os_code);
            db.OSInfoList.Add(os_info);

            var cmd = $"insert into divisions (division_id, os_id, name, code) values ({division_id},{os_id},'{name}','{os_code}')";
            db.Execute(cmd);

            return os_code;
        }
    }

    public class DB
    {
        int count_divisions = 4;
        static string connect_line = $"Server=localhost; database=laba7; UID=root; password={File.ReadAllText("dbpass.txt")}";
        MySqlConnection connection = new MySqlConnection(connect_line);

        public List<OSInfo> OSInfoList = new List<OSInfo>();
        public Dictionary<int, Division> divisions = new Dictionary<int, Division>();

        public DB()
        {
            connection.Open();
            for (int i = 0; i < count_divisions; i++)
            {
                divisions[i + 1] = new Division(this, i + 1);
            }

            LoadOSList();
        }

        public void LoadOSList()
        {
            var reader = ExecuteReader("select * from divisions");

            while(reader.Read())
            {
                var div_id = reader.GetInt32(0);
                var name = reader.GetString(2);
                var code = reader.GetString(3);

                var div = divisions[div_id];
                var os_info = new OSInfo(name, code);
                OSInfoList.Add(os_info);
                div.count_os++;
            }

            reader.Close();
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
