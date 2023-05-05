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

namespace MAI_Laba11
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string main_template = "import math\n\ndef f1(x):\r\n    return math.sqrt(x)\n\ndef f2(x):\r\n    return 1 / x\n\ndef f3(x):\n    return math.exp(x)";
        public MainWindow()
        {
            InitializeComponent();
        }

        public double Func(double x, char id)
        {
            switch(id)
            {
                case '1':
                    return Math.Sqrt(x);

                case '2':
                    return 1.0 / x;

                case '3':
                    return Math.Exp(x);

                default: return 0.0;
            }
        }

        public double ResFunc(double x, string pattern)
        {
            for(int i = 0; i < pattern.Length; i++)
            {
                x = Func(x, pattern[pattern.Length-i-1]);
            }

            return x;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(double.TryParse(XValue.Text, out var x))
            {
                var pattern = SelectedPattern.SelectedItem as string;

                if(pattern == null)
                {
                    MessageBox.Show("Укажите последовательность!");
                    return;
                }
                
                var y = ResFunc(x, pattern);

                if (double.IsNaN(y))
                {
                    MessageBox.Show("Вне области определения!");
                    return;
                }

                ResultY.Content = $"y = {y}";
            }
            else
            {
                MessageBox.Show("Неправильный формат!");
            }
        }

        private void SelectedPattern_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var pattern = SelectedPattern.SelectedItem as string;
            FuncPreview.Content = $"y = F{pattern![0]}(F{pattern[1]}(F{pattern[2]}(x)))";

            string code = $"{main_template}\n\nx = float(input())\ny = f{pattern[0]}(f{pattern[1]}(f{pattern[2]}(x)))\n\nprint(y)";
            ResultCode.Text = code;
        }
    }
}
