using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using TradeApp.Model;
using System.Linq;

namespace TradeApp.windows
{
    public partial class CustomCalendar : UserControl
    {
        private Grid calendarGrid;
        private ObservableCollection<Trade> trades;

        public CustomCalendar(ObservableCollection<Trade> trades)
        {
            this.trades = trades;
            InitializeCalendar();
        }

        private void InitializeCalendar()
        {
            calendarGrid = new Grid
            {
                Background = new SolidColorBrush(Color.FromRgb(30, 30, 47)),
                Margin = new Thickness(10)
            };

            for (int i = 0; i < 7; i++)
                calendarGrid.ColumnDefinitions.Add(new ColumnDefinition());

            for (int i = 0; i < 6; i++)
                calendarGrid.RowDefinitions.Add(new RowDefinition());

            GenerateCalendar(DateTime.Now.Year, DateTime.Now.Month);

            Content = calendarGrid;
        }

        public void GenerateCalendar(int year, int month)
        {
            calendarGrid.Children.Clear();

            // הוספת כפתורי ניווט
            StackPanel navigationPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(10) };

            Button prevMonthButton = new Button { Content = "<", Width = 40, Height = 30, Margin = new Thickness(5) };
            prevMonthButton.Click += (s, e) => GenerateCalendar(month == 1 ? year - 1 : year, month == 1 ? 12 : month - 1);

            TextBlock monthYearText = new TextBlock
            {
                Text = $"{new DateTime(year, month, 1):MMMM yyyy}",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10),
                Width = 200,
                TextAlignment = TextAlignment.Center
            };

            Button nextMonthButton = new Button { Content = ">", Width = 40, Height = 30, Margin = new Thickness(5) };
            nextMonthButton.Click += (s, e) => GenerateCalendar(month == 12 ? year + 1 : year, month == 12 ? 1 : month + 1);

            navigationPanel.Children.Add(prevMonthButton);
            navigationPanel.Children.Add(monthYearText);
            navigationPanel.Children.Add(nextMonthButton);

            Grid.SetRow(navigationPanel, 0);
            Grid.SetColumnSpan(navigationPanel, 7);
            calendarGrid.Children.Add(navigationPanel);

            // הוספת כותרות הימים (ראשון עד שבת)
            string[] dayNames = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
            for (int j = 0; j < 7; j++)
            {
                TextBlock dayName = new TextBlock
                {
                    Text = dayNames[j],
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.White,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 5, 0, 5)
                };
                Grid.SetRow(dayName, 1);
                Grid.SetColumn(dayName, j);
                calendarGrid.Children.Add(dayName);
            }

            DateTime firstDay = new DateTime(year, month, 1);
            int daysInMonth = DateTime.DaysInMonth(year, month);
            int startDay = (int)firstDay.DayOfWeek;

            int dayCounter = 1;

            // וידוא שהרשת כוללת מספיק שורות לחודשים עם 6 שבועות
            while (calendarGrid.RowDefinitions.Count < 8)
            {
                calendarGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            }

            for (int i = 2; i < 8; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (i == 2 && j < startDay) continue;
                    if (dayCounter > daysInMonth) break;

                    Border border = new Border
                    {
                        BorderBrush = Brushes.LightGray,
                        BorderThickness = new Thickness(2),
                        Margin = new Thickness(5),
                        Background = new SolidColorBrush(Color.FromRgb(20, 20, 30))
                    };

                    StackPanel dayPanel = new StackPanel
                    {
                        Orientation = Orientation.Vertical,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    TextBlock dateText = new TextBlock
                    {
                        Text = dayCounter.ToString(),
                        FontSize = 20,
                        FontWeight = FontWeights.Bold,
                        Foreground = Brushes.White,
                        Margin = new Thickness(0, 5, 0, 5)
                    };

                    // חישוב ה-PnL ליום זה
                    var pnlForDay = trades.Where(t => t.ExitDateTime.HasValue && t.ExitDateTime.Value.Date == new DateTime(year, month, dayCounter)).Sum(t => t.ProfitLoss);

                    TextBlock profitText = new TextBlock
                    {
                        Text = pnlForDay != 0 ? $"${pnlForDay}" : "",
                        FontSize = 16,
                        Foreground = pnlForDay >= 0 ? Brushes.LightGreen : Brushes.LightCoral
                    };

                    dayPanel.Children.Add(dateText);
                    dayPanel.Children.Add(profitText);

                    border.Child = dayPanel;

                    Grid.SetColumn(border, j);
                    Grid.SetRow(border, i);

                    calendarGrid.Children.Add(border);

                    dayCounter++;
                }
            }
        }
    }
}
