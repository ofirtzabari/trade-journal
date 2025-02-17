using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;
using TradeApp.Model;

namespace TradeApp.Windows
{
    public class ChartsComponent : UserControl
    {
        private StackPanel chartsPanel;
        private ObservableCollection<Trade> trades;
        private int chartHeight = 700;

        public ChartsComponent(ObservableCollection<Trade> trades)
        {
            this.trades = trades;
            InitializeCharts();
            GenerateCumulativePnLChart();
            GenerateCombinedTradeTypeChart();
        }

        private void InitializeCharts()
        {
            chartsPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Background = new SolidColorBrush(Color.FromRgb(30, 30, 47)),
                Margin = new Thickness(10)
            };

            var scrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                Content = chartsPanel,
            };

            Content = scrollViewer;
        }

        public void GenerateCumulativePnLChart()
        {
            chartsPanel.Children.Clear();

            var title = new TextBlock
            {
                Text = "Cumulative PnL Over Time",
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10, 0, 20)
            };
            chartsPanel.Children.Add(title);

            var cumulativePnLByDate = trades
                .Where(t => t.ExitDateTime.HasValue)
                .GroupBy(t => t.ExitDateTime.Value.Date)
                .OrderBy(g => g.Key)
                .Select(g => new { Date = g.Key, CumulativePnL = g.Sum(t => t.ProfitLoss) })
                .ToList();

            double runningTotal = 0;
            var chartValues = new ChartValues<double>();
            var dates = new System.Collections.Generic.List<string>();

            foreach (var entry in cumulativePnLByDate)
            {
                runningTotal += entry.CumulativePnL;
                chartValues.Add(runningTotal);
                dates.Add(entry.Date.ToShortDateString());
            }

            var lineSeries = new LineSeries
            {
                Title = "Cumulative PnL",
                Values = chartValues,
                Stroke = Brushes.LightGreen,
                Fill = Brushes.Transparent,
                PointGeometrySize = 5
            };

            var cartesianChart = new CartesianChart
            {
                Height = chartHeight,
                Series = new SeriesCollection { lineSeries },
                AxisX = { new Axis { Title = "Date", Labels = dates, Separator = new LiveCharts.Wpf.Separator { IsEnabled = false } } },
                AxisY = { new Axis { Title = "PnL", LabelFormatter = value => value.ToString("C"), Separator = new LiveCharts.Wpf.Separator { IsEnabled = false }, Sections = { new AxisSection { Value = 0, Stroke = Brushes.Red, StrokeThickness = 2 } } } }
            };

            chartsPanel.Children.Add(cartesianChart);
        }

        public void GenerateCombinedTradeTypeChart()
        {
            var title = new TextBlock
            {
                Text = "Trades Type And Seccess",
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 20, 0, 20)
            };
            chartsPanel.Children.Add(title);

            int shortTrades = trades.Count(t => t.PositionType == true);
            int longTrades = trades.Count(t => t.PositionType == false);

            int positiveShortTrades = trades.Count(t => t.PositionType == true && t.ProfitLoss > 0);
            int positiveLongTrades = trades.Count(t => t.PositionType == false && t.ProfitLoss > 0);

            var seriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Long Trades",
                    Values = new ChartValues<int> { longTrades },
                    Fill = Brushes.Green,
                },
                new ColumnSeries
                {
                    Title = "Short Trades",
                    Values = new ChartValues<int> { shortTrades },
                    Fill = Brushes.Red,
                },
                new ColumnSeries
                {
                    Title = "",
                    Values = new ChartValues<int> { 0 },
                    Fill = Brushes.Transparent,
                    MaxColumnWidth = 150,
                    DataLabels = false,
                    Foreground = Brushes.Transparent
                },
                new ColumnSeries
                {
                    Title = "Positive Long Trades",
                    Values = new ChartValues<int> { positiveLongTrades },
                    Fill = Brushes.LightGreen,
                },
                new ColumnSeries
                {
                    Title = "Positive Short Trades",
                    Values = new ChartValues<int> { positiveShortTrades },
                    Fill = Brushes.Orange,
                }
            };

            var cartesianChart = new CartesianChart
            {
                Height = chartHeight,
                Series = seriesCollection,
                AxisX = { new Axis { Title = "Trade Type", Labels = new System.Collections.Generic.List<string> { "Long", "", "Short" } } },
                AxisY = { new Axis { Title = "Number of Trades", LabelFormatter = value => ((int)value).ToString(), Separator = new LiveCharts.Wpf.Separator { Step = 1, IsEnabled = true } } }
            };

            chartsPanel.Children.Add(cartesianChart);
        }
    }
}
