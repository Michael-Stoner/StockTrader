using StockTrader;
using StockTrader.Approaches;
using StockTrader.Approaches.Run;
using System;
using System.Collections.Generic;
using System.Text;

namespace StocksTracker.Plugins.Buy
{
    [Serializable]
    public class DaysOfRun : IApproachAlgorithm
    {
        private int daysRun { get; set; }

        public string Description { get { return "Days Of Run"; } }
        public string Abbreviation { get { return "Run"; } }

        public void InitializeRun(RunAlgorithmDefinition definition)
        {
            daysRun = (int)definition.GetParameter("DaysRun").NumericValue;

        }

        public bool Buy(StockData[] dailyStockPrices, int currentDay, out int buyDay, out Order order)
        {
            buyDay = -1;
            order = null;

            var startDay = (currentDay - (daysRun - 1));

            if (currentDay == dailyStockPrices.GetUpperBound(0)) return false;  //if we are at the end of data, don't buy to prevent false numbers
            if (startDay < 0) return false; //can't check data we don't have

            for (int day = startDay; day <= currentDay; day++)
            {
                if (dailyStockPrices[day].Close < dailyStockPrices[day].Open)
                {
                    //this is a down day, so the run has failed
                    return false;
                }
            }

            //if we get here, we have a successful run and we trigger a buy
            //we are buying the next day, so we advance the buy day
            buyDay = currentDay + 1;

            order = new Order
            {
                Reason = $"{Abbreviation} {daysRun}",
                Price = dailyStockPrices[buyDay].Open,
                TransactionDay = buyDay,
            };

            return true;
        }

        public AlgorithmDefinition GetDefaultDefinition(bool sellDefinition = false)
        {
            if (sellDefinition)
            {
                return null;
            }
            else
            {
                return new AlgorithmDefinition
                {
                    SellDefinition = false,
                    Name = "DaysRun",
                    Description = "Days Run will stop trading after a number of positive days of growth defined by the 'DaysRun' parameter",
                    Parameters = new List<AlgorithmParameter>()
                {
                    new AlgorithmParameter
                    {
                        Name ="DaysRun",
                        NumericValue = 1,
                        UseRange = true,
                        RangeStart = 1,
                        RangeEnd = 5,
                        RangeStep=1,
                    }
                }
                };
            }
        }

        public bool Sell(StockData[] dailyStockPrices, int purchaseDay, int currentDay, in Order buyOrder, out int sellDay, out Order order)
        {
            //This algorithm is only a buy algoritm.  This should never get hit due to returning null for sell the definition.
            throw new NotImplementedException("DaysRun is a buy algorithm only");
        }

    }
}