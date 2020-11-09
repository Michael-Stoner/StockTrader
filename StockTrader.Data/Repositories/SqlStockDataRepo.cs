using StockTrader.Data.Downloader;
using StockTrader.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StockTrader.Data.Repositories
{
    public class SqlStockDataRepo : IStockDataRepo
    {
        private readonly StockTraderContext _stockTraderContext;
        private readonly IStockDownloader _stockDownloader;

        private readonly DateTime DefaultStartDate = new DateTime(1988, 01, 01);  //The start of electronic trading
        private readonly DateTime defaultEndDate = DateTime.Now;

        public SqlStockDataRepo(StockTraderContext stockTraderContext, IStockDownloader stockDownloader)
        {
            _stockTraderContext = stockTraderContext;
            _stockDownloader = stockDownloader;

        }

        private IEnumerable<StockData> Get(string ticker, DateTime startDate, DateTime endDate, bool skipDownload = true)
        {
            //Check the database
            var stocks = _stockTraderContext.StockData.Where(s =>
                s.Ticker.Equals(ticker) &&
                s.Date >= startDate &&
                s.Date <= endDate).OrderBy(s => s.Date).ToList();

            //If it doesn't exist in the database, then try to download data
            if (stocks.Count == 0 && !skipDownload)
            {
                //download
                var success = _stockDownloader.Download(this, ticker, startDate, endDate);

                //if was downloaded, we get the records for return
                if (success)
                    return Get(ticker, startDate, endDate, skipDownload: true);
            }

            return stocks;
        }

        public IEnumerable<StockData> Get(string ticker, DateTime startDate, DateTime endDate)
        {
            return Get(ticker, startDate, endDate, false);
        }

        public IEnumerable<StockData> Get(string ticker)
        {
            return Get(ticker, DefaultStartDate, defaultEndDate, false);
        }

        public IEnumerable<StockData> GetAll()
        {
            return _stockTraderContext.StockData.ToList();
        }

        public void Add(StockData stock)
        {
            _stockTraderContext.Add(stock);
        }

        public void Add(IEnumerable<StockData> stocks)
        {
            _stockTraderContext.AddRange(stocks);
        }

        public bool SaveChanges()
        {
            return (_stockTraderContext.SaveChanges() >= 0);
        }

        public IEnumerable<Stock> GetAllStocks()
        {
            return _stockTraderContext.Stocks.OrderBy(s => s.Ticker).ToList();
        }

        public Stock GetStock(string ticker)
        {
            return _stockTraderContext.Stocks.FirstOrDefault(s => s.Ticker.Equals(ticker, StringComparison.OrdinalIgnoreCase));

        }

        public IEnumerable<Stock> QueryStocks(string query)
        {
            return _stockTraderContext.Stocks.Where(s =>
                        s.Ticker.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                        s.Name.Contains(query, StringComparison.OrdinalIgnoreCase)
                        );

        }
    }
}