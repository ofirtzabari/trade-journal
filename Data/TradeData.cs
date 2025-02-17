using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TradeApp.Model;

namespace TradeApp.Data
{
    public static class TradeData
    {
        public static void AddTradeToDb(Trade trade)
        {
            using (var db = new TradeContext())
            {
                db.Add(trade);
                db.SaveChanges();
            }
        }
        public static List<Trade> GetTradeList() 
        {
            using (var db = new TradeContext())
            {
                return db.Trades.ToList();
            }
        }
        public static void DeleteTrade(Trade trade)
        {
            using (var db = new TradeContext())
            {
                var tradeToDelete = db.Trades.FirstOrDefault(t => t.Id == trade.Id);
                if (tradeToDelete != null)
                {
                    db.Trades.Remove(tradeToDelete);
                    db.SaveChanges();
                }
            }
        }
        public static void UpdateTradeInDb(Trade trade)
        {
            using (var db = new TradeContext())
            {
                db.Trades.Update(trade);
                db.SaveChanges();
            }
        }
    }
}
