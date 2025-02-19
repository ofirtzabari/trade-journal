using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TradeApp.Model
{
    public class PlaybookContext : DbContext
    {
        public DbSet<Playbook> Playbooks { get; set; }
        public string path = @"C:\SRC\TradeApp\TradeApp\DataBase\Playbooks.db";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlite($"Data Source={path}");
    }
}
