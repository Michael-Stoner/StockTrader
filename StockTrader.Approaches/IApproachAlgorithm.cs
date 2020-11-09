using StockTrader.Approaches.Run;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockTrader.Approaches
{
    public interface IApproachAlgorithm
    {

        public bool Buy(StockData[] dailyStockPrices, int currentDay, out int buyDay, out Order order);

        public bool Sell(StockData[] dailyStockPrices, int purchaseDay, int currentDay, in Order buyOrder, out int sellDay, out Order order);

        public void InitializeRun(RunAlgorithmDefinition definition);

        public AlgorithmDefinition GetDefaultDefinition(bool sellDefinition = false);

    }
}