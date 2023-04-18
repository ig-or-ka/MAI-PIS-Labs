using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MAI_Laba1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DB connection = new DB();

        public MainWindow()
        {
            InitializeComponent();
            connection.UpdateEvent += UpdateMakeOrderGrid;

            MakeOrderGrid.ItemsSource = connection.Products;
            ProductsGrid.ItemsSource = connection.Products;
            ClientsGrid.ItemsSource = connection.Clients;

            var clients_items = new List<ComboBoxItem>();

            foreach(var client in connection.Clients)
            {
                var item = new ComboBoxItem();
                item.Content= client.Name;
                item.Tag = client;
                clients_items.Add(item);
            }

            SelectedClient.ItemsSource = clients_items;
        }

        bool UpdateOn = true;
        public void UpdateMakeOrderGrid()
        {
            if (UpdateOn)
            {
                MakeOrderGrid.ItemsSource = null;
                MakeOrderGrid.ItemsSource = connection.Products;

                int AllSumOreder = 0;
                foreach (var product in connection.Products)
                {
                    if (product.Selected)
                    {
                        AllSumOreder -= product.SumPrice;
                    }
                    else
                    {
                        AllSumOreder += product.SumPrice;
                    }                    
                }
                OrederSum.Content = AllSumOreder;

                ClientBalance.Content = connection.CurrentClient?.Balance;
                ClientSumPays.Content = connection.CurrentClient?.Sum;
                ClientDebt.Content = connection.CurrentClient?.Debt;

                ClientsGrid.ItemsSource = null;
                ClientsGrid.ItemsSource = connection.Clients;

                ProductsGrid.ItemsSource = null;
                ProductsGrid.ItemsSource = connection.Products;
            }            
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            int price = ProductPrice.Text == "" ? 0 : int.Parse(ProductPrice.Text);
            int count = ProductCount.Text == "" ? 0 : int.Parse(ProductCount.Text);
            connection.NewProduct(ProductName.Text, price, count);

            ProductsGrid.ItemsSource = null;
            ProductsGrid.ItemsSource = connection.Products;            
        }

        private void SearchClient_TextChanged(object sender, TextChangedEventArgs e)
        {
            var name = ((TextBox)e.Source).Text;

            var list_clients = new List<Client>();
            var all_clients = connection.Clients;

            foreach (Client client in all_clients)
            {
                if (client.Name.Contains(name))
                {
                    list_clients.Add(client);
                }                
            }

            ClientsGrid.ItemsSource = null;
            ClientsGrid.ItemsSource = list_clients;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected_item = (ComboBoxItem)TypeBuy.SelectedItem;

            if(selected_item.Content != null)
            {
                var type_oreder = selected_item.Content as string;
                if (type_oreder == "Взаимозачет" || type_oreder == "Бартер")
                {
                    if(type_oreder == "Взаимозачет")
                    {
                        connection.CurrentOrderType = OrederTypes.PayToPay;
                    }
                    else
                    {
                        connection.CurrentOrderType = OrederTypes.Barter;
                    }

                    MakeOrderGrid.Columns[0].Visibility = Visibility.Visible;
                }
                else
                {
                    if (type_oreder == "Наличный расчет")
                    {
                        connection.CurrentOrderType = OrederTypes.Cash;
                    }
                    else if (type_oreder == "Безналичный расчет")
                    {
                        connection.CurrentOrderType = OrederTypes.Balance;
                    }
                    else
                    {
                        connection.CurrentOrderType = OrederTypes.Debt;
                    }

                    MakeOrderGrid.Columns[0].Visibility = Visibility.Hidden;
                }

                UpdateOn = false;
                foreach(var product in connection.Products)
                {
                    product.Selected = false;
                    product.CountToBuy = 0;
                }
                UpdateOn = true;
                UpdateMakeOrderGrid();
            }            
        }

        private void SelectedClient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected_item = (ComboBoxItem)SelectedClient.SelectedItem;
            var selected_client = (Client)selected_item.Tag;
            connection.CurrentClient = selected_client;

            UpdateMakeOrderGrid();
        }

        private void MakeOrder_Click(object sender, RoutedEventArgs e)
        {
            if(connection.CurrentClient== null)
            {
                MessageBox.Show("Выберите клиента!");
                return;
            }

            int AllSumOreder = 0;
            foreach (var product in connection.Products)
            {
                if (product.Selected)
                {
                    AllSumOreder -= product.SumPrice;
                }
                else
                {
                    AllSumOreder += product.SumPrice;
                }
            }

            if(connection.CurrentOrderType == OrederTypes.Cash 
                || connection.CurrentOrderType == OrederTypes.Balance
                || connection.CurrentOrderType == OrederTypes.Debt)
            {
                if(connection.CurrentOrderType == OrederTypes.Balance)
                {
                    if (connection.CurrentClient.Balance < AllSumOreder)
                    {
                        MessageBox.Show("Недостаточно средств для оплаты!");
                        return;
                    }

                    connection.CurrentClient.Balance -= AllSumOreder;
                }
                else if(connection.CurrentOrderType == OrederTypes.Debt)
                {
                    if (connection.CurrentClient.Balance < AllSumOreder)
                    {
                        int sum_debt = AllSumOreder - connection.CurrentClient.Balance;
                        if(sum_debt > connection.CurrentClient.MaxCredit)
                        {
                            MessageBox.Show("Нельзя превысить потолок кредита!");
                            return;
                        }

                        connection.CurrentClient.Balance = 0;
                        connection.CurrentClient.Debt += sum_debt;
                        connection.CurrentClient.MaxCredit -= sum_debt;
                    }
                    else
                    {
                        connection.CurrentClient.Balance -= AllSumOreder;
                    }
                }

                connection.CurrentClient.Sum += AllSumOreder;
                connection.CurrentClient.Save();

                UpdateOn = false;
                foreach (var product in connection.Products)
                {
                    product.Count -= product.CountToBuy;
                    product.Save();

                    product.Selected = false;
                    product.CountToBuy = 0;
                }
                UpdateOn = true;
                UpdateMakeOrderGrid();
            }
            else
            {
                if(connection.CurrentOrderType == OrederTypes.Barter)
                {
                    if(AllSumOreder != 0)
                    {
                        MessageBox.Show("Произведите равноценный обмен!");
                        return;
                    }                    
                }
                else
                {
                    if(AllSumOreder > 0)
                    {
                        MessageBox.Show("Сумма внесенного товара должна быть больше суммы купленного!");
                        return;
                    }
                    connection.CurrentClient.Debt += AllSumOreder;
                    connection.CurrentClient.MaxCredit -= AllSumOreder;
                    connection.CurrentClient.Save();
                }

                UpdateOn = false;
                foreach (var product in connection.Products)
                {
                    if (product.Selected)
                    {
                        product.Count += product.CountToBuy;
                    }
                    else
                    {
                        product.Count -= product.CountToBuy;
                    }                    
                    product.Save();

                    product.Selected = false;
                    product.CountToBuy = 0;
                }
                UpdateOn = true;
                UpdateMakeOrderGrid();
            }

            MessageBox.Show("Заказ создан!");
        }
    }
}
