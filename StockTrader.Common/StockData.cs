using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StockTrader
{
    public class StockData
    {

        public int Id { get; set; }

        public string Ticker { get; set; }

        public DateTime Date { get; set; }

        public double Open { get; set; }

        public double High { get; set; }

        public double Close { get; set; }

        public double Low { get; set; }

        public long Volume { get; set; }

        public double GetPrice(PriceType priceType)
        {
            switch (priceType)
            {
                case PriceType.Open:
                    return Open;

                case PriceType.Close:
                    return Close;

                case PriceType.High:
                    return High;

                case PriceType.Low:
                    return Low;

                default:
                    return Close;
            }
        }
    }
}