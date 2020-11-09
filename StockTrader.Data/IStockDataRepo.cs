using StockTrader.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockTrader.Data
{
    public interface IStockDataRepo
    {
        bool SaveChanges();

        IEnumerable<StockData> GetAll();

        IEnumerable<StockData> Get(string ticker);

        IEnumerable<StockData> Get(string ticker, DateTime startDate, DateTime endDate);

        IEnumerable<Stock> GetAllStocks();

        Stock GetStock(string ticker);

        IEnumerable<Stock> QueryStocks(string query);

        void Add(StockData stock);

        void Add(IEnumerable<StockData> stocks);

    }
}