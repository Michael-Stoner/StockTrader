using AutoMapper;
using CsvHelper;
using StockTrader.Data.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace StockTrader.Data.Downloader.Yahoo
{
    public class YahooFinanceDownloader : IStockDownloader
    {
        private static IMapper _mapper = (new MapperConfiguration(cfg => cfg.CreateMap<CsvStock, StockData>())).CreateMapper();

        public bool Download(IStockDataRepo repo, string ticker, DateTime startDate, DateTime endDate)
        {

            //TODO: Validate ticker symbol before attempting download

            var start = new DateTimeOffset(startDate).ToUnixTimeSeconds();
            var end = new DateTimeOffset(endDate).ToUnixTimeSeconds();    //DateTimeOffset.Now.ToUnixTimeSeconds();

            string yahooUrl = $"https://query1.finance.yahoo.com/v7/finance/download/{ticker}?period1={start}&period2={end}&interval=1d&events=history";

            using (var stocksStream = (new WebClient()).OpenRead(yahooUrl))
            using (var csvReader = new CsvReader(new StreamReader(stocksStream), CultureInfo.InvariantCulture))
            {
                var fileRecords = csvReader.GetRecords<CsvStock>().ToList();

                fileRecords.ForEach(s => s.Ticker = ticker);

                var stockData = _mapper.Map<List<StockData>>(fileRecords);

                repo.Add(stockData);
                return repo.SaveChanges();
            }

        }

        public bool Download(string ticker)
        {
            throw new NotImplementedException();
        }
    }
}