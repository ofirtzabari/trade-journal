# 📈 TradeApp

TradeApp is a comprehensive trading journal application built using C# and WPF. It helps traders track and analyze their trades, providing valuable insights through an interactive dashboard and calendar.

## 🚀 Features
- **Trade Journal**: Log all trades with essential details.
- **Interactive Calendar**: Visualize trades by their closing dates.
- **Dashboard Metrics**:
  - Total PnL
  - Profit Factor
  - Win Rate
  - Average PnL
  - Average Trade Duration
- **Responsive UI**: Intuitive and clean interface for easy navigation.

## 🛠️ Tech Stack
- **C#**: Backend logic
- **WPF**: User interface
- **SQLite**: Database management
- **Entity Framework Core**: ORM for database interactions
- **Git**: Version control

## 🏗️ Installation
1. Clone the repository:
    ```bash
    git clone https://github.com/yourusername/TradeApp.git
    ```
2. Open the solution in Visual Studio.
3. Run the following commands to apply database migrations:
    ```bash
    Add-Migration Initial
    Update-Database
    ```
4. Build and run the application.

## 🖼️ Application Overview
### Main Window
The main window includes:
- **Add Trade** button to add new trades.
- **Dashboard** for trade insights.
- **My Trades** section for reviewing all trades.
- **Calendar** view for analyzing trades over time.

### Dashboard
Displays real-time statistics about trading performance.

### Calendar
Interactive calendar with daily trade performance.

### Trade List
Comprehensive view of all trades with options to edit or delete individual trades.

## ⚙️ Database Schema
The application uses SQLite with the following structure:
- **Trades** table containing columns:
  - `Id`: Primary Key
  - `Ticker`: Stock ticker symbol
  - `Quantity`: Number of shares/contracts
  - `EnterDateTime` & `ExitDateTime`: Trade entry and exit timestamps
  - `EnterPrice` & `ExitPrice`: Entry and exit prices
  - `PositionType`: `true` for long, `false` for short
  - `ProfitLoss`: Calculated PnL
  - `TradeDuration`: Time duration of the trade

## 🛠️ Contributing
1. Fork the repository.
2. Create a new branch.
3. Make your changes.
4. Submit a pull request.

## ⚠️ Troubleshooting
- **Database Issues**: Run migrations again if tables are missing.
- **UI Glitches**: Clean and rebuild the project.

## 📜 License
Licensed under the MIT License.

---

Happy Trading! 📊💹
