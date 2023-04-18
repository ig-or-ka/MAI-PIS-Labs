using System.ComponentModel;
using System.Windows;

namespace Laba1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DB? connection;

        public MainWindow()
        {
            connection = new DB();
            //connection.PrintUsers();

            InitializeComponent();
            Closing += OnClose;
        }

        public void OnClose(object? sender, CancelEventArgs e)
        {
            connection?.Close();
        }

        private void login_button_Click(object sender, RoutedEventArgs e)
        {
            var user = connection?.AuthUser(name_box.Text, pass_box.Password);

            if (user != null)
            {
                var wind = new EditWindow(connection!, user);
                connection = null;
                wind.Show();
                Close();
            }
            else
            {
                MessageBox.Show("Неправильные данные!");
            }
        }

        private void Guest_Click(object sender, RoutedEventArgs e)
        {
            var wind = new EditWindow(connection!, null);
            connection = null;
            wind.Show();
            Close();
        }
    }
}
