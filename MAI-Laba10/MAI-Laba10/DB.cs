using MySqlConnector;
using System.Collections.Generic;
using System.IO;

namespace MAI_Laba10
{
    public class ProductInfo
    {
        public int Code { get => _code; }
        public string Name { get => _name; }
        public int Count { get => _count; }
        public int Price { get => _price; }
        public string Date { get => _date; }
        public int AllSum { get => _count* _price; }

        int _code;
        string _name;
        int _price;
        public int _count = 0;        
        public string _date = "";

        public ProductInfo(int code, string name, int price) {
            _code = code;
            _name = name;
            _price = price;
        }
    }

    public class DB
    {
        static string connect_line = $"Server=localhost; database=laba10; UID=root; password={File.ReadAllText("dbpass.txt")}";
        MySqlConnection connection = new MySqlConnection(connect_line);

        public List<ProductInfo> ProductsInfoList = new List<ProductInfo>();

        public DB()
        {
            connection.Open();
        }

        public void Load(string date)
        {
            ProductsInfoList.Clear();

            var products = new Dictionary<int, ProductInfo>();

            var reader = ExecuteReader("select * from products");

            while(reader.Read())
            {
                var id = reader.GetInt32(0);
                var name = reader.GetString(1);
                var price = reader.GetInt32(2);

                var product = new ProductInfo(id, name, price);
                products[id] = product;
                ProductsInfoList.Add(product);

            }

            reader.Close();

            var cmd = "select * from availability";
            if(date != "")
            {
                cmd += $" where date =  '{date}'";
            }
            reader = ExecuteReader(cmd);

            while (reader.Read())
            { 
                var prod_id = reader.GetInt32(0);
                var count = reader.GetInt32(2);
                var date_prod = reader.GetString(3);

                var product = products[prod_id];
                product._date = date_prod;
                product._count += count;
            }

            reader.Close();

            foreach(var prod in products.Values)
            {
                if(prod.Count == 0)
                {
                    ProductsInfoList.Remove(prod);
                }
            }
        }

        public MySqlDataReader ExecuteReader(string sql)
        {
            var cmd = new MySqlCommand(sql, connection);
            return cmd.ExecuteReader();
        }

        public void Close() => connection.Close();
    }
}
