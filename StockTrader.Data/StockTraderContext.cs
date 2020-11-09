using Microsoft.EntityFrameworkCore;
using StockTrader.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockTrader.Data
{
    public class StockTraderContext : DbContext
    {
        public StockTraderContext(DbContextOptions<StockTraderContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        public DbSet<Stock> Stocks { get; set; }
        public DbSet<StockData> StockData { get; set; }

        public DbSet<Approach> Approaches { get; set; }

        public DbSet<Run> Runs { get; set; }
        //public DbSet<RunDefinitionJson> RunDefinitionJsons { get; set; }

    }
}