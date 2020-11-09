using StockTrader;
using StockTrader.Approaches;
using StockTrader.Approaches.Run;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stocks.Approaches.Sell
{
    [Serializable]
    public class StopLoss : IApproachAlgorithm
    {
        private double stopLossPercent { get; set; }
        private bool useClose { get; set; }

        public string Description { get { return "Stop Loss"; } }
        public string Abbreviation { get { return "SL"; } }

        public bool UseDayHigh { get; } = false;
        public bool UseDayLow { get; } = true;

        public void InitializeRun(RunAlgorithmDefinition approachDefinition)
        {
            stopLossPercent = approachDefinition.GetParameter("StopLossPercent").NumericValue;

            useClose = approachDefinition.GetParameter("UseClose").BooleanValue;
        }

        public bool Sell(StockData[] dailyStockPrices, int purchaseDay, int currentDay, in Order buyOrder, out int sellDay, out Order order)
        {
            double lowValue = useClose ? dailyStockPrices[currentDay].Close : dailyStockPrices[currentDay].Low;

            double currentLoss = Utility.CalculatePercent(buyOrder.Price, lowValue);

            if (currentLoss < 0 && Math.Abs(currentLoss).GreaterThanAlmostEqualTo(stopLossPercent))
            {
                sellDay = currentDay + 1;

                order = new Order()
                {
                    Reason = $"SL: > {Math.Round(stopLossPercent * 100, 2)}%",
                    Price = useClose ? lowValue : buyOrder.Price * (1 - stopLossPercent),
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
                    Name = "StopLoss",
                    Parameters = new List<AlgorithmParameter>()
                {
                    new AlgorithmParameter
                    {
                        Name ="StopLossPercent",
                        UseRange = true,
                        NumericValue = 0.01,
                        RangeStart = 0.01,
                        RangeEnd = 0.10,
                        RangeStep = 0.01,

                    },
                    new AlgorithmParameter
                    {
                        Name ="UseClose",
                        BooleanValue = true,
                        ValueIsBoolean = true,
                        UseRange = false,     //flip between true and false

                    }
                }
                };
            }
        }

        public bool Buy(StockData[] dailyStockPrices, int currentDay, out int buyDay, out Order order)
        {
            //This algorithm is only a sell algoritm.  This should never get hit due to returning null for buy the definition.
            throw new NotImplementedException("StopLoss is only a sell algorithm.");
        }
    }
}