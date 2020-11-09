using StockTrader;
using StockTrader.Approaches;
using StockTrader.Approaches.Run;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stocks.Approaches.Sell
{
    [Serializable]
    public class SellThreshold : IApproachAlgorithm
    {
        private double HighThreshold { get; set; }
        private bool useClose { get; set; }

        public string Description { get; } = "Profit Threshold";
        public string Abbreviation { get; } = "PT";

        public void InitializeRun(RunAlgorithmDefinition approachDefinition)
        {
            HighThreshold = approachDefinition.GetParameter("HighThreshold").NumericValue;
            useClose = approachDefinition.GetParameter("UseClose").BooleanValue;
        }

        public bool Sell(StockData[] dailyStockPrices, int purchaseDay, int currentDay, in Order buyOrder, out int sellDay, out Order order)
        {
            var profit = Utility.CalculatePercent(buyOrder.Price, dailyStockPrices[currentDay].High);

            if (profit > 0 && profit > HighThreshold)
            {
                sellDay = currentDay;
                order = new Order()
                {
                    Reason = $"{Abbreviation} {Math.Round(profit * 100, 2)}%",
                    Price = buyOrder.Price * (1 + HighThreshold),
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
                    Name = "SellThreshold",
                    Parameters = new List<AlgorithmParameter>()
                {
                    new AlgorithmParameter
                    {
                        Name ="HighThreshold",
                        NumericValue = .05,
                        RangeStart = 0.01,
                        RangeEnd = 0.10
                    },
                    new AlgorithmParameter
                    {
                        Name ="UseClose",
                        BooleanValue = false,
                        UseRange = true,  //flip between true and false

                    }
                }
                };
            }
        }

        public bool Buy(StockData[] dailyStockPrices, int currentDay, out int buyDay, out Order order)
        {
            //This algorithm is only a sell algoritm.  This should never get hit due to returning null for buy the definition.
            throw new NotImplementedException("SellThreashold is only a sell algorithm.");
        }
    }
}