using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace MAI_Laba7
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DB connection = new DB();
        public MainWindow()
        {
            InitializeComponent();
            Closing += OnClose;
            OSGrid.ItemsSource = connection.OSInfoList;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(DivisionID.Text == "" || OSName.Text == "")
            {
                MessageBox.Show("Введите все данные!");
                return;
            }

            if(int.TryParse(DivisionID.Text, out int div_id))
            {
                if(connection.divisions.ContainsKey(div_id))
                {
                    var code = connection.divisions[div_id].CreateOS(OSName.Text);
                    ResCode.Text = $"Код внутреннего учета: {code}";

                    OSGrid.ItemsSource = null;
                    OSGrid.ItemsSource = connection.OSInfoList;
                }
                else
                {
                    MessageBox.Show("Некорректный номер подразделения!");
                }
            }
            else
            {
                MessageBox.Show("Неправильный формат!");
            }
        }
        public void OnClose(object? sender, CancelEventArgs e)
        {
            connection.Close();
        }
    }
}
