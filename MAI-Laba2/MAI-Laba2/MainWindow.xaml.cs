using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace MAI_Laba2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        PlotModel MainModel = new PlotModel();
        PlotModel HistModel = new PlotModel();
        List<double> Nums = new List<double>();

        public MainWindow()
        {
            InitializeComponent();

            MainModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = 0, Maximum = 100 });
            MainModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Minimum = 0, Maximum = 30 });

            var barSeries = new BarSeries { ItemsSource = new List<BarItem> { new BarItem(0) } };
            var barAxes = new CategoryAxis { Position = AxisPosition.Left, ItemsSource = new string[] { "1" } };

            HistModel.Series.Add(barSeries);
            HistModel.Axes.Add(barAxes);

            MainPlotView.Model = MainModel;
            HistPlotView.Model = HistModel;
        }

        private void Enter_Click(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var num = double.Parse(Digits.Text);
                Digits.Text = "";

                Nums.Add(num);

                RedrawModels(Nums);

                var numsStr = "Вы ввели числа:";

                if(Nums.Count > 1)
                {
                    var lastNum = Nums[Nums.Count-2];                    
                    var perRaz = Math.Abs(lastNum - num) / lastNum * 100;

                    if (perRaz > 40)
                    {
                        MessageBox.Show($"Число {num} отличается от предыдущего ({lastNum}) более чем на 40% (на {Math.Round(perRaz,2)})");
                    }
                }                


                foreach(var a in Nums)
                {
                    numsStr += " " + a;
                }

                InputedNumbers.Text = numsStr;
            }            
        }

        private void RedrawModels(List<double> nums)
        {
            MainModel.Series.Clear();
            MainModel.Axes.Clear();
            HistModel.Series.Clear();
            HistModel.Axes.Clear();

            MainModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = 0, Maximum = nums.Max() + 20 });
            MainModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Minimum = 0, Maximum = nums.Count + 1 });

            var lineSeries = new LineSeries();

            var seriesList = new List<BarItem>();
            var axesList = new List<int>();
            
            for (int i = 0; i < nums.Count; i++)
            {
                lineSeries.Points.Add(new DataPoint(i + 1, nums[i]));
                seriesList.Add(new BarItem { Value = nums[i] });
                axesList.Add(i + 1);
            }

            double sumAll = nums.Sum() / nums.Count;
            var sredLine = new LineSeries { Color = OxyColor.Parse("#cc0000") };
            sredLine.Points.Add(new DataPoint(1, sumAll));
            sredLine.Points.Add(new DataPoint(nums.Count, sumAll));

            MainModel.Series.Add(lineSeries);
            MainModel.Series.Add(sredLine);

            var barSeries = new BarSeries { ItemsSource = seriesList };
            var barAxes = new CategoryAxis { Position = AxisPosition.Left, ItemsSource = axesList };

            HistModel.Series.Add(barSeries);
            HistModel.Axes.Add(barAxes);

            MainModel.InvalidatePlot(true);
            HistModel.InvalidatePlot(true);
        }

        private void SelectNum_Click(object sender, RoutedEventArgs e)
        {
            var selectedNums = new List<double>();

            for(int i = 0; i < Nums.Count; i++)
            {
                if(FromNum.Text != "" && int.Parse(FromNum.Text) - 1 > i)
                {
                    continue;
                }

                if (ToNum.Text != "" && int.Parse(ToNum.Text) - 1 < i)
                {
                    continue;
                }

                if (KratNum.Text != "" && Nums[i] % double.Parse(KratNum.Text) != 0)
                {
                    continue;
                }

                selectedNums.Add(Nums[i]);
            }

            RedrawModels(selectedNums);
        }
    }
}
