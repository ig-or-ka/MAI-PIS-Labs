using System.Text;
using System.Windows;
using System.Net.Sockets;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace Laba3_Client
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Socket socket;        

        public MainWindow()
        {
            InitializeComponent();

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect("localhost", 8536);            
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/111.0.0.0 Safari/537.36");
                client.DefaultRequestHeaders.Add("accept", "*/*");
                client.DefaultRequestHeaders.Add("x-requested-with", "XMLHttpRequest");
                var url = $"https://bankiros.ru/currency/get-chat-rate-history-cbrf?block=view&currency_id=1&period={Period.Text}";
                var response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();


                var text = response.Content.ReadAsStringAsync().Result;
                var result_json = JArray.Parse(text);

                var cmd = "enter:";
                foreach (JObject item in result_json)
                {
                    cmd += $":{item["value"].ToString().Replace(",", ".")}";
                }
                cmd += "\n";
                socket.Send(Encoding.UTF8.GetBytes(cmd));
            }
            catch
            {
                MessageBox.Show("Ошибка получения данных!");
            }            
        }

        private void Clean_Click(object sender, RoutedEventArgs e)
        {
            socket.Send(Encoding.UTF8.GetBytes("clean\n"));
        }
    }
}
