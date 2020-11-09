using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockTrader.Data.Downloader.Yahoo
{
    public class CsvStock
    {
        [Ignore]
        public string Ticker { get; set; }

        public DateTime Date { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }

        [Name("Adj Close")]
        public double AdjustedClose { get; set; }

        public long Volume { get; set; }

    }
}