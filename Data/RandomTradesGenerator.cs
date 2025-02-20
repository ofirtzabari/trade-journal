using System;
using System.Collections.Generic;
using System.IO;
using TradeApp.Model;
using TradeApp.Data;
using System.Collections.ObjectModel;
using TradeApp.Context;

public static class RandomTradesGenerator
{
    private static Random random = new Random();

    public static void GenerateRandomTrades(int count)
    {
        var tickers = new string[] { "AAPL", "GOOG", "TSLA", "MSFT", "AMZN", "NFLX", "META", "NVDA" };

        using (var db = new TradeContext())
        {
            for (int i = 0; i < count; i++)
            {
                var enterDate = GetRandomDate();
                var exitDate = GetRandomDateAfter(enterDate);
                float enterPrice = (float)Math.Round(random.NextDouble() * 300 + 50, 2);
                float exitPrice = (float)Math.Round(random.NextDouble() * 300 + 50, 2);
                float quantity = random.Next(1, 100);
                bool positionType = random.Next(2) == 0; // true = long, false = short
                bool isOpen = random.Next(2) == 0;

                if (isOpen)
                {
                    exitDate = null;
                    exitPrice = 0;
                }

                var trade = new Trade(
                    tickers[random.Next(tickers.Length)],
                    quantity,
                    enterDate,
                    exitDate,
                    enterPrice,
                    exitPrice,
                    positionType,
                    isOpen
                );

                db.Trades.Add(trade);
            }

            db.SaveChanges();
        }
    }

    private static DateTime GetRandomDate()
    {
        int year = DateTime.Now.Year;
        int month = random.Next(1, 13);
        int day = random.Next(1, 28);
        int hour = random.Next(0, 24);
        int minute = random.Next(0, 60);
        int second = random.Next(0, 60);
        return new DateTime(year, month, day, hour, minute, second);
    }

    private static DateTime? GetRandomDateAfter(DateTime startDate)
    {
        if (startDate == default) return null;

        int daysToAdd = random.Next(1, 30);
        int hour = random.Next(0, 24);
        int minute = random.Next(0, 60);
        int second = random.Next(0, 60);
        return startDate.AddDays(daysToAdd).AddHours(hour).AddMinutes(minute).AddSeconds(second);
    }
}


