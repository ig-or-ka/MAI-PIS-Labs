using System.ComponentModel;
using System.Windows;

namespace MAI_Laba10
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
            Closing += OnClose;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            connection.Load(DateTextBox.Text);
            ProductsGrid.ItemsSource = null;
            ProductsGrid.ItemsSource = connection.ProductsInfoList;
        }
        public void OnClose(object? sender, CancelEventArgs e)
        {
            connection.Close();
        }
    }
}
