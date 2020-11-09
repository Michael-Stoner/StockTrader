using System;
using System.Collections.Generic;
using System.Text;

namespace StockTrader.Data.Downloader
{
    public interface IStockDownloader
    {
        //bool Download(string ticker);
        bool Download(IStockDataRepo repo, string ticker, DateTime startDate, DateTime endDate);

    }
}