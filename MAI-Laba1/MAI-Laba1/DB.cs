using MySqlConnector;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;

namespace MAI_Laba1
{
    public enum OrederTypes
    {
        Cash, Balance, Debt, Barter, PayToPay
    }
    public class Product
    {
        public bool Selected { 
            get => _Selected;
            set
            {
                _Selected = value;
                if(!value)
                {
                    _CountToBuy = 0;
                    connection.UpdateEvent.DynamicInvoke();
                }
            }
        }
        public string Name { get => _Name; }
        public int Count { get => _Count; set => _Count = value; }
        public int Price { get => _Price; set => _Price = value; }
        public int CountToBuy { 
            get => _CountToBuy; 
            set {
                if((connection.CurrentOrderType != OrederTypes.Barter && connection.CurrentOrderType != OrederTypes.PayToPay) || !Selected)
                {
                    if (value > Count)
                    {
                        MessageBox.Show("Товара на складе недостаточно!");
                        return;
                    }
                }
                _CountToBuy = value; 
                connection.UpdateEvent.DynamicInvoke(); 
            } 
        }
        public int SumPrice { get => _Price * _CountToBuy; }

        int _ID = -1;
        string _Name = "";
        bool _Selected = false;
        int _Count = 0;
        int _Price = 0;
        int _CountToBuy = 0;
        DB connection;

        public Product(int ID, MySqlDataReader reader, DB connection)
        {
            _ID = ID;
            _Name = reader.GetString(1);
            _Price = reader.GetInt32(2);
            _Count = reader.GetInt32(3);
            this.connection = connection;
        }

        public void Save()
        {
            connection.Execute($"update Products set name='{_Name}', price={_Price}, count={_Count} where id = {_ID}");
        }
    }

    public class Client
    {
        public string Name { get => _Name; set { _Name = value; modify = true; } }
        public int Sum { get => _Sum; set { _Sum = value; modify = true; } }
        public int Balance { get => _Balance; set { _Balance = value; modify = true; } }
        public int MaxCredit { get => _MaxCredit; set { _MaxCredit = value; modify = true; } }
        public int Debt { get => _Debt; set { _Debt = value; modify = true; } }
        public int RemainingDebt { get => _MaxCredit - Debt; }
        public string Comment { get => _Comment; set { _Comment = value; modify = true; } }

        int _ID = -1;
        string _Name = "";
        int _Sum = 0;
        int _Balance = 0;
        int _MaxCredit = 0;
        int _Debt = 0;
        string _Comment = "";
        bool modify = false;
        DB connection;

        public Client(int ID, MySqlDataReader reader, DB connection)
        {
            _ID = ID;
            _Name = reader.GetString(1);
            _Sum = reader.GetInt32(2);
            _Balance = reader.GetInt32(3);
            _MaxCredit = reader.GetInt32(4);
            _Debt = reader.GetInt32(5);
            _Comment = reader.GetString(6);
            this.connection = connection;
        }

        public void Save()
        {
            connection.Execute($"update Сustomers set name='{_Name}', sum={_Sum}, balance={_Balance}, max_credit={_MaxCredit}, debt={_Debt}, comment='{_Comment}' where id = {_ID}");
        }
    }

    public class DB
    {
        public delegate void UpdateEventDelegate();
        public UpdateEventDelegate UpdateEvent;
        public OrederTypes CurrentOrderType = OrederTypes.Cash;
        public Client? CurrentClient = null;

        static string connect_line = $"Server=localhost; database=laba1; UID=root; password={File.ReadAllText("dbpass.txt")}";
        MySqlConnection connection = new MySqlConnection(connect_line);

        public Dictionary<int, Product> _Products = new Dictionary<int, Product>();
        public Dictionary<string, Product> _ProductsByName = new Dictionary<string, Product>();
        List<Product> _ProductsList = new List<Product>();

        public Dictionary<int, Client> _Clients = new Dictionary<int, Client>();
        List<Client> _ClientsList = new List<Client>();

        public DB()
        {
            connection.Open();
            ReadProduct();
            ReaClients();
        }

        public Product[] Products
        {
            get
            {
                var res = new Product[_ProductsList.Count];
                _ProductsList.CopyTo(res);
                return res;
            }
        }

        public Client[] Clients
        {
            get
            {
                var res = new Client[_ClientsList.Count];
                _ClientsList.CopyTo(res);
                return res;
            }
        }

        public Product? NewProduct(string name, int price, int count)
        {
            Product? product = null;

            if (_ProductsByName.ContainsKey(name))
            {
                product = _ProductsByName[name];
                if (count > 0)
                {
                    product.Count = count;
                }
                if (price > 0)
                {
                    product.Price = price;
                }

                product.Save();
            }
            else
            {
                Execute($"insert into Products (name, price, count) values ('{name}', {price}, {count})");
                var reader = ExecuteReader($"select * from Products where name = '{name}' and price = {price} and count = {count}");

                if (reader.Read())
                {
                    var ID = reader.GetInt32(0);
                    product = new Product(ID, reader, this);
                    _Products[ID] = product;
                    _ProductsByName[product.Name] = product;
                    _ProductsList.Add(product);
                }

                reader.Close();
            }            

            return product;
        }

        void ReadProduct()
        {
            var reader = ExecuteReader($"select * from Products");

            while (reader.Read())
            {
                var ID = reader.GetInt32(0);
                var product = new Product(ID, reader, this);
                _Products[ID] = product;
                _ProductsByName[product.Name] = product;
                _ProductsList.Add(product);
            }

            reader.Close();
        }

        void ReaClients()
        {
            var reader = ExecuteReader($"select * from Сustomers");

            while (reader.Read())
            {
                var ID = reader.GetInt32(0);
                var client = new Client(ID, reader, this);
                _Clients[ID] = client;
                _ClientsList.Add(client);
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
