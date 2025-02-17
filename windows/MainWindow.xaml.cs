using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TradeApp.Data;
using TradeApp.Model;
using TradeApp.windows;
using TradeApp.Windows;

namespace TradeApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<Trade> trades;
        private CustomCalendar tradeCalendar;
        private Dashboard dashboard;
        private ChartsComponent charts;


        public MainWindow()
        {
            InitializeComponent();
            //RandomTradesGenerator.GenerateRandomTrades(10);
            WindowState = WindowState.Maximized;
            trades = new ObservableCollection<Trade>(TradeData.GetTradeList());
            TradesListView.ItemsSource = trades;
            AddCalendar();
            AddDashboard();
            AddCharts();
        }

        private void RefreshMainWindow()
        {
            // refresh dashboard
            dashboard?.UpdateStatistics();
            // refresh calendar
            tradeCalendar?.GenerateCalendar(DateTime.Now.Year, DateTime.Now.Month);
            // refresh charts
            charts?.GenerateCumulativePnLChart();
            charts?.GenerateCombinedTradeTypeChart();
        }

        private void new_trade(object sender, RoutedEventArgs e)
        {
            WindowNewTrade w = new WindowNewTrade(trades);
            bool? result = w.ShowDialog();
            if (result == true)
                RefreshMainWindow();
        }

        private void DashBoard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            HideRightSection();
            dashboard.Visibility = Visibility.Visible;
        }

        private void MyTrades_MouseDown(object sender, MouseButtonEventArgs e)
        {
            HideRightSection();
            TradesPanel.Visibility = Visibility.Visible;
        }

        private void Calander_MouseDown(object sender, MouseButtonEventArgs e)
        {
            HideRightSection();
            tradeCalendar.Visibility = Visibility.Visible;

        }
        private void Chart_MouseDown(object sender, MouseButtonEventArgs e)
        {
            HideRightSection();
            charts.Visibility = Visibility.Visible;
        }
        private void HideRightSection()
        {
            foreach (UIElement child in optionGrid.Children)
            {
                if (Grid.GetColumn(child) == 1)
                {
                    child.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void AddCalendar()
        {
            tradeCalendar = new CustomCalendar(trades);
            tradeCalendar.GenerateCalendar(DateTime.Now.Year, DateTime.Now.Month);
            Grid.SetColumn(tradeCalendar, 1);
            Grid.SetRow(tradeCalendar, 1);
            tradeCalendar.Visibility = Visibility.Collapsed;
            optionGrid.Children.Add(tradeCalendar);
        }

        private void AddDashboard()
        {
            dashboard = new Dashboard(trades);
            Grid.SetColumn(dashboard, 1);
            Grid.SetRow(dashboard, 1);
            dashboard.Visibility = Visibility.Collapsed;
            optionGrid.Children.Add(dashboard);
        }
        private void AddCharts()
        {
            charts = new ChartsComponent(trades);
            Grid.SetColumn(charts, 1);
            Grid.SetRow(charts, 1);
            charts.Visibility = Visibility.Collapsed;
            optionGrid.Children.Add(charts);
        }

        private void EditTrade_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Trade tradeToEdit)
            {
                var editWindow = new WindowNewTrade(trades, tradeToEdit);
                if (editWindow.ShowDialog() == true)
                {
                    TradeData.UpdateTradeInDb(tradeToEdit);
                    var existing = trades.FirstOrDefault(t => t.Id == tradeToEdit.Id);
                    if (existing != null)
                    {
                        existing.Ticker = tradeToEdit.Ticker;
                        existing.Quantity = tradeToEdit.Quantity;
                        existing.EnterPrice = tradeToEdit.EnterPrice;
                        existing.ExitPrice = tradeToEdit.ExitPrice;
                        existing.EnterDateTime = tradeToEdit.EnterDateTime;
                        existing.ExitDateTime = tradeToEdit.ExitDateTime;
                        existing.PositionType = tradeToEdit.PositionType;
                        RefreshMainWindow();
                    }
                }
            }
        }

        private void DeleteTrade_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Trade tradeToDelete)
            {
                // אישור מחיקה מהמשתמש
                var result = MessageBox.Show($"Are you sure you want to delete trade: {tradeToDelete.Ticker}?",
                                             "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    // מחיקת הטרייד מהמסד
                    TradeData.DeleteTrade(tradeToDelete);

                    var tradeInList = trades.FirstOrDefault(t => t.Id == tradeToDelete.Id);
                    if (tradeInList != null)
                    {
                        trades.Remove(tradeInList);
                    }

                    // ריענון הנתונים במסך
                    RefreshMainWindow();
                    MessageBox.Show("Trade deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Error: Trade not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}