using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace StockTrader.Indicators
{
    public class IndicatorManager
    {
        public static double[] GetSimpleMovingAverage(StockData[] dailyStockPrices, PriceType priceType, int days, int indexStart, int indexEnd)
        {
            double[] simpleMovingAverage;

            var stockName = dailyStockPrices.First().Ticker;

            //lock (cacheLockObject)
            //{
            if (!GetCachedMovingAverage(stockName, PriceType.Close, (int)days, out simpleMovingAverage))
            {
                var key = GetSmaKey(stockName, priceType, days);

                //calculate the entire moving average for all the stock prices so that it can be cached.
                simpleMovingAverage = SimpleMovingAverage.Calculate(dailyStockPrices, PriceType.Close, (int)days, 0, (dailyStockPrices.Count() - 1));
                //cache the result
                PutCachedMovingAverage(stockName, PriceType.Close, (int)days, simpleMovingAverage);

            }

            //}

            return simpleMovingAverage;
        }

        private static Dictionary<string, double[]> cachedMovingAverages = new Dictionary<string, double[]>();
        private static Object cacheLockObject = new List<string>();

        private static bool GetCachedMovingAverage(string stockName, PriceType priceType, int days, out double[] movingAverage)
        {
            var key = GetSmaKey(stockName, priceType, days);

            if (cachedMovingAverages.ContainsKey(key))
            {
                movingAverage = cachedMovingAverages[key];
                return true;
            }
            else
            {
                movingAverage = null;
                return false;
            }
        }

        private static bool PutCachedMovingAverage(string stockName, PriceType priceType, int days, double[] movingAverage)
        {
            var key = GetSmaKey(stockName, priceType, days);

            lock (cachedMovingAverages)
            {
                if (cachedMovingAverages.ContainsKey(key))
                {
                    //Utility.OutputQueue.Enqueue($"Moving Average Cache Clash:  {key}");
                    return true;   //since it is multi-threaded, it is possible 2 threads calculated and tried to cache.  In that case, we just ignore the request
                }

                cachedMovingAverages.Add(key, movingAverage);
            }

            return true;

        }

        private static string GetSmaKey(string stockName, PriceType priceType, int days)
        {
            return $"{stockName}_{days}days_{priceType}";
        }
    }
}