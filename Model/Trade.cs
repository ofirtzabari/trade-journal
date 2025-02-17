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

        // עדכון חישוב PnL בהתאם לסוג הפוזיציה
        public float ProfitLoss => PositionType == false // שינוי לבדיקה נכונה
            ? (ExitPrice - EnterPrice) * Quantity
            : (EnterPrice - ExitPrice) * Quantity;

        // עדכון חישוב אחוז ההחזר בהתאם לסוג הפוזיציה
        public float ReturnPercentage => EnterPrice != 0
            ? PositionType == false // שינוי לבדיקה נכונה
                ? ((ExitPrice - EnterPrice) / EnterPrice) * 100
                : ((EnterPrice - ExitPrice) / EnterPrice) * 100
            : 0;

        public string PositionTypeDescription => PositionType == false ? "Long" : "Short"; // עדכון תיאור נכון

        public TimeSpan? TradeDuration => (EnterDateTime.HasValue && ExitDateTime.HasValue)
        ? ExitDateTime - EnterDateTime
        : null;

        public Trade(string ticker, float quantity, DateTime? enterDateTime, DateTime? exitDateTime, float enterPrice, float exitPrice, bool? positionType)
        {
            Ticker = ticker;
            Quantity = quantity;
            EnterDateTime = enterDateTime;
            ExitDateTime = exitDateTime;
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
