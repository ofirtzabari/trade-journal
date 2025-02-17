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
using System.Windows.Shapes;
using TradeApp.Data;
using TradeApp.Model;

namespace TradeApp
{
    /// <summary>
    /// Interaction logic for WindowNewTrade.xaml
    /// </summary>
    public partial class WindowNewTrade : Window
    {
        private ObservableCollection<Trade> trades;
        private Trade existingTrade;
        private bool isEditMode;

        public WindowNewTrade(ObservableCollection<Trade> trades)
        {
            this.trades = trades;
            InitializeComponent();
            CenterWindowOnScreen();
            isEditMode = false;
        }

        // Constructor for editing an existing trade
        public WindowNewTrade(ObservableCollection<Trade> trades, Trade tradeToEdit) : this(trades)
        {
            existingTrade = tradeToEdit;
            isEditMode = true;
            LoadTradeData(tradeToEdit);
            this.Title = "Edit Trade";
            saveButton.Content = "Save";
        }

        private void CenterWindowOnScreen()
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;
            this.Left = (screenWidth / 2) - (windowWidth / 2);
            this.Top = (screenHeight / 2) - (windowHeight / 2);
        }

        private void save_trade(object sender, RoutedEventArgs e)
        {
            try
            {
                string ticker = ticker_textbox.Text;
                DateTime? enterDT = enterDateTime.Value;
                DateTime? exitDT = exitDateTime.Value;
                float enterPrice = float.Parse(enterPrice_textbox.Text);
                float exitPrice = float.Parse(exitPrice_textbox.Text);
                float quantity = float.Parse(quantity_textbox.Text);
                bool? positionType = positionToggle.IsChecked;

                if (isEditMode && existingTrade != null)
                {
                    // Update existing trade
                    existingTrade.Ticker = ticker;
                    existingTrade.EnterDateTime = enterDT;
                    existingTrade.ExitDateTime = exitDT;
                    existingTrade.EnterPrice = enterPrice;
                    existingTrade.ExitPrice = exitPrice;
                    existingTrade.Quantity = quantity;
                    existingTrade.PositionType = positionType;

                    TradeData.UpdateTradeInDb(existingTrade);
                }
                else
                {
                    // Create new trade
                    Trade t = new(ticker, quantity, enterDT, exitDT, enterPrice, exitPrice, positionType);
                    TradeData.AddTradeToDb(t);
                    trades.Add(t);
                }
                RefreshTradeList();
                this.DialogResult = true;
                this.Close();
            }
            catch
            {
                MessageBox.Show("One or more of the fields are not valid");
            }
        }

        private void RefreshTradeList()
        {
            // עדכון הרשימה באופן מיידי
            trades.Clear();
            var updatedTrades = TradeData.GetTradeList();
            foreach (var trade in updatedTrades)
            {
                trades.Add(trade);
            }
        }

        private void LoadTradeData(Trade trade)
        {
            ticker_textbox.Text = trade.Ticker;
            enterDateTime.Value = trade.EnterDateTime;
            exitDateTime.Value = trade.ExitDateTime;
            enterPrice_textbox.Text = trade.EnterPrice.ToString();
            exitPrice_textbox.Text = trade.ExitPrice.ToString();
            quantity_textbox.Text = trade.Quantity.ToString();
            positionToggle.IsChecked = trade.PositionType;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void positionToggle_Checked(object sender, RoutedEventArgs e)
        {
            positionToggle.Content = "Short";
            positionToggle.Foreground = Brushes.Red;
            positionToggle.Background = Brushes.LightPink;
        }

        private void positionToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            positionToggle.Content = "Long";
            positionToggle.Foreground = Brushes.Green;
            positionToggle.Background = Brushes.LightGray;
        }
    }
}
