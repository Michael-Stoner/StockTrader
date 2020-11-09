using StockTrader.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using StockTrader;

//using static Stocks.Utility;

namespace StockTrader.Indicators
{
    internal class SimpleMovingAverage
    {
        public static double[] Calculate(StockData[] dailyStockPrices, PriceType priceType, int days, int indexStart, int indexEnd)
        {
            var startOfRange = (indexStart) - (days - 1);  //the days include the day being calculated (so it's 1 less day)

            //If the index is too close to the start of data, then we have to adjust the start
            if (startOfRange < 0)
            {
                startOfRange += Math.Abs(startOfRange);
                indexStart = (days - 1);
            }

            double[] smas = new double[indexEnd + 1];

            double sma = 0;
            for (int i = startOfRange; i <= indexEnd; i++)
            {
                sma += dailyStockPrices[i].GetPrice(priceType);

                if (i == indexStart)
                {
                    smas[i] = sma / days;
                }
                else if (i > indexStart)
                {
                    sma -= dailyStockPrices[i - days].GetPrice(priceType);
                    smas[i] = Math.Round(sma / days, 2);
                }

            }

            return smas;

            //return smas.Skip(indexStart).Take(indexEnd - indexStart).ToArray();

        }

    }
}