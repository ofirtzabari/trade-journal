using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TradeApp.Model;

namespace TradeApp.Windows
{
    public class Dashboard : UserControl
    {
        private Grid dashboardGrid;
        private ObservableCollection<Trade> trades;

        public Dashboard(ObservableCollection<Trade> trades)
        {
            this.trades = trades;
            InitializeDashboard();
            UpdateStatistics();
        }

        private void InitializeDashboard()
        {
            dashboardGrid = new Grid
            {
                Background = new SolidColorBrush(Color.FromRgb(30, 30, 47)),
                Margin = new Thickness(10)
            };

            for (int i = 0; i < 5; i++) // שינוי ל-5 כדי להוסיף שורה נוספת
            {
                dashboardGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            }

            dashboardGrid.ColumnDefinitions.Add(new ColumnDefinition());
            Content = dashboardGrid;
        }

        public void UpdateStatistics()
        {
            dashboardGrid.Children.Clear();

            double totalPnL = trades.Sum(t => t.ProfitLoss);
            double profitFactor = trades.Where(t => t.ProfitLoss > 0).Sum(t => t.ProfitLoss) /
                                      Math.Max(1, Math.Abs(trades.Where(t => t.ProfitLoss < 0).Sum(t => t.ProfitLoss)));
            double winRate = trades.Count > 0 ? (double)trades.Count(t => t.ProfitLoss > 0) / trades.Count * 100 : 0;
            double averagePnL = trades.Count > 0 ? trades.Average(t => t.ProfitLoss) : 0;
            double averageDuration = trades.Count > 0 ? trades.Where(t => t.TradeDuration.HasValue).Average(t => t.TradeDuration.Value.TotalDays) : 0;

            var stats = new (string Label, string Value)[]
            {
                ("Total PnL", $"{totalPnL:C}"),
                ("Profit Factor", profitFactor.ToString("F2")),
                ("Win Rate", $"{winRate:F2}%"),
                ("Average PnL", $"{averagePnL:C}"),
                ("Avg Trade Duration", $"{averageDuration:F2} days")
            };

            for (int i = 0; i < stats.Length; i++)
            {
                var border = new Border
                {
                    BorderBrush = Brushes.LightGray,
                    BorderThickness = new Thickness(2),
                    Background = new SolidColorBrush(Color.FromRgb(45, 45, 60)),
                    CornerRadius = new CornerRadius(10),
                    Margin = new Thickness(5),
                    Padding = new Thickness(10)
                };

                var stackPanel = new StackPanel { Orientation = Orientation.Vertical, HorizontalAlignment = HorizontalAlignment.Center };

                var label = new TextBlock
                {
                    Text = stats[i].Label,
                    Foreground = Brushes.White,
                    FontWeight = FontWeights.Bold,
                    FontSize = 18,
                    Margin = new Thickness(5)
                };

                var value = new TextBlock
                {
                    Text = stats[i].Value,
                    Foreground = Brushes.LightGreen,
                    FontSize = 24,
                    FontWeight = FontWeights.SemiBold,
                    Margin = new Thickness(5)
                };

                stackPanel.Children.Add(label);
                stackPanel.Children.Add(value);
                border.Child = stackPanel;

                Grid.SetRow(border, i);
                dashboardGrid.Children.Add(border);
            }
        }
    }
}
