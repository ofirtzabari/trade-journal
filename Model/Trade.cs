using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeApp.Model
{
    public class Trade
    {
        private DateTime enterDate;
        private DateTime? exitDate;

        [Key] // הגדרת מפתח ראשי
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-increment
        public int Id { get; set; }

        public string Ticker { get; set; }

        public float Quantity { get; set; }

        public DateTime? EnterDateTime { get; set; }

        public DateTime? ExitDateTime { get; set; }

        public float EnterPrice { get; set; }

        public float ExitPrice { get; set; }

        public bool? PositionType { get; set; }

        public bool IsOpen { get; set; }

        public float ProfitLoss => IsOpen
            ? 0 // עסקה פתוחה עדיין לא נסגרה ולכן אין רווח/הפסד
            : (PositionType == false
                ? (ExitPrice - EnterPrice) * Quantity
                : (EnterPrice - ExitPrice) * Quantity);

        // עדכון חישוב אחוז ההחזר בהתאם לסוג הפוזיציה והאם היא פתוחה
        public float ReturnPercentage => IsOpen || EnterPrice == 0
            ? 0 // עסקה פתוחה או מחיר כניסה לא תקף -> החזר 0%
            : (PositionType == false
                ? ((ExitPrice - EnterPrice) / EnterPrice) * 100
                : ((EnterPrice - ExitPrice) / EnterPrice) * 100);

        public string PositionTypeDescription => PositionType == false ? "Long" : "Short";

        public string DisplayExitPrice => IsOpen ? "" : ExitPrice.ToString("F2");

        
        public string DisplayProfitLoss => IsOpen ? "" : ProfitLoss.ToString("F2");

        public TimeSpan? TradeDuration => (EnterDateTime.HasValue && ExitDateTime.HasValue)
        ? ExitDateTime - EnterDateTime
        : null;

        public string TradeStatus => IsOpen ? "Open" : "Closed";

        public Trade(string ticker, float quantity, DateTime? enterDateTime, DateTime? exitDateTime, float enterPrice, float exitPrice, bool? positionType, bool isOpen)
        {
            Ticker = ticker;
            Quantity = quantity;
            EnterDateTime = enterDateTime;
            ExitDateTime = exitDateTime;
            EnterPrice = enterPrice;
            ExitPrice = exitPrice;
            PositionType = positionType;
            IsOpen = isOpen;
        }

        public Trade(string ticker, float quantity, DateTime enterDate, DateTime? exitDate, float enterPrice, float exitPrice, bool positionType)
        {
            Ticker = ticker;
            Quantity = quantity;
            this.enterDate = enterDate;
            this.exitDate = exitDate;
            EnterPrice = enterPrice;
            ExitPrice = exitPrice;
            PositionType = positionType;
        }

        public override string ToString()
        {
            return $"Trade: {Ticker} | {PositionType} | Qty: {Quantity} | Enter: {EnterPrice} | Exit: {ExitPrice} | PnL: {ProfitLoss:C}";
        }

    }
}
