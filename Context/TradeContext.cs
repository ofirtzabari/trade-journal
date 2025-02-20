using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TradeApp.Model;

namespace TradeApp.Context
{
    public class TradeContext : DbContext
    {
        public DbSet<Trade> Trades { get; set; }
        public string path = @"C:\SRC\TradeApp\TradeApp\DataBase\Trades.db";
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlite($"Data Source={path}");
    }
}
