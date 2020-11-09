using StockTrader;
using StockTrader.Approaches;
using StockTrader.Approaches.Run;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stocks.Approaches.Sell
{
    [Serializable]
    public class TrailingStop : IApproachAlgorithm
    {
        private double trailingStopPercent { get; set; }
        private bool useClose { get; set; } = false;
        private int daysToLimitHigh { get; set; } = -1;

        public string Description { get; } = "Trailing Stop";
        public string Abbreviation { get; } = "TS";

        public void InitializeRun(RunAlgorithmDefinition approachDefinition)
        {
            trailingStopPercent = approachDefinition.GetParameter("TrailingStopPercent").NumericValue;
            useClose = approachDefinition.GetParameter("UseClose").BooleanValue;
        }

        public bool Sell(StockData[] dailyStockPrices, int purchaseDay, int currentDay, in Order buyOrder, out int sellDay, out Order order)
        {
            double high = 0;

            for (int day = purchaseDay; day <= currentDay; day++)
            {
                var highValue = useClose ? dailyStockPrices[day].Close : dailyStockPrices[day].High;

                if (highValue > high)
                {
                    high = highValue;
                }
            }

            double lowValue = useClose ? dailyStockPrices[currentDay].Close : dailyStockPrices[currentDay].Low;

            double currentLoss = Utility.CalculatePercent(high, lowValue);

            if (currentLoss < 0 && Math.Abs(currentLoss).GreaterThanAlmostEqualTo(trailingStopPercent))
            {
                //it closed 2 percent down from the high, so it would have triggered a sell on that day
                sellDay = currentDay;

                order = new Order()
                {
                    Reason = $"{Abbreviation} {Math.Round(currentLoss * 100, 2)}%  H: {high:C}  {trailingStopPercent}",
                    Price = useClose ? lowValue : high * (1 - trailingStopPercent),
                    TransactionDay = sellDay,
                };

                return true;
            }

            sellDay = -1;
            order = null;
            return false;

        }

        public AlgorithmDefinition GetDefaultDefinition(bool sellDefinition = true)
        {
            if (!sellDefinition)
            {
                return null;
            }
            else
            {
                return new AlgorithmDefinition
                {
                    Name = "TrailingStop",
                    Parameters = new List<AlgorithmParameter>()
                {
                    new AlgorithmParameter
                    {
                        Name ="TrailingStopPercent",
                        UseRange = true,
                        RangeStart = 0.01,
                        RangeEnd = 0.10,
                        RangeStep = 0.005,
                    },
                    new AlgorithmParameter
                    {
                        Name ="UseClose",
                        BooleanValue = true,
                        ValueIsBoolean = true,
                        UseRange = false,  //flip between true and false

                    }

                }
                };
            }
        }

        public bool Buy(StockData[] dailyStockPrices, int currentDay, out int buyDay, out Order order)
        {
            //This algorithm is only a sell algoritm.  This should never get hit due to returning null for buy the definition.
            throw new NotImplementedException("TrailingStop is only a sell algorithm.");
        }

    }
}