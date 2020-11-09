using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using StockTrader.Approaches;
using StockTrader;
using StockTrader.Indicators;

using StockTrader.Approaches.Run;

namespace Stocks.Approaches.BuySell
{
    [Serializable]
    internal class SimpleMovingAverage : IApproachAlgorithm
    {
        [JsonIgnore]
        private static ConcurrentDictionary<string, Order[]> localSmaCache = new ConcurrentDictionary<string, Order[]>();

        private double daysInMovingAverage { get; set; }
        private double daysInLookback { get; set; }
        private bool triggerUp { get; set; }

        public string Description { get; } = "Simple Moving Average";
        public string Abbreviation { get; } = "SMA";

        public bool Buy(StockData[] dailyStockPrices, int currentDay, out int buyDay, out Order order)
        {
            var key = GenerateKey(dailyStockPrices);

            //check the cache to see if the results are cached
            if (localSmaCache.ContainsKey(key))
            {
                //they are cached, so we return the order
                var orders = localSmaCache[key];

                order = orders[currentDay];
                buyDay = order?.TransactionDay ?? 0;

                return order != null;
            }
            else
            {
                //cache and get the result
                BuildOrdersCache(dailyStockPrices);
                return Buy(dailyStockPrices, currentDay, out buyDay, out order);
            }

            //return BuySell(dailyStockPrices, currentDay, out buyDay, out order);
        }

        public bool Sell(StockData[] dailyStockPrices, int purchaseDay, int currentDay, in Order buyOrder, out int sellDay, out Order order)
        {
            var key = GenerateKey(dailyStockPrices);

            //check the cache to see if the results are cached
            if (localSmaCache.ContainsKey(key))
            {
                //they are cached, so we return the order
                var orders = localSmaCache[key];

                order = orders[currentDay];
                sellDay = order?.TransactionDay ?? 0;

                return order != null;
            }
            else
            {
                //cache and get the result
                BuildOrdersCache(dailyStockPrices);
                return Sell(dailyStockPrices, purchaseDay, currentDay, in buyOrder, out sellDay, out order);
            }

            //return BuySell(dailyStockPrices, currentDay, out sellDay, out order);
        }

        private string GenerateKey(StockData[] dailyStockPrices)
        {
            return $"{dailyStockPrices.First().Ticker}_{Abbreviation}_{daysInMovingAverage}_{daysInLookback}_{triggerUp}";
        }

        private void BuildOrdersCache(StockData[] dailyStockPrices)
        {
            var key = GenerateKey(dailyStockPrices);

            Order[] orders = new Order[dailyStockPrices.Count()];

            for (int i = 0; i < dailyStockPrices.Length; i++)
            {
                int transactionDay;
                Order order;

                if (BuySell(dailyStockPrices, i, out transactionDay, out order))
                {
                    orders[i] = order;
                }
            }

            //if (!SmaCache.ContainsKey(key))
            localSmaCache.TryAdd(key, orders);

        }

        private bool BuySell(StockData[] dailyStockPrices, int currentDay, out int transactionDay, out Order order)
        {
            transactionDay = 0;
            order = null;

            //if (daysInLookback > daysInMovingAverage) return false;  //this happens because of ranges.
            if (currentDay == dailyStockPrices.GetUpperBound(0)) return false;
            if (currentDay < (int)daysInLookback) return false;

            double[] simpleMovingAverage = IndicatorManager.GetSimpleMovingAverage(dailyStockPrices, PriceType.Close, (int)daysInMovingAverage, 0, (dailyStockPrices.Count() - 1));

            //if the currentday is 0 then a moving average couldn't be calculated due to it's proximity to the start of the array
            if (simpleMovingAverage[currentDay] == 0) return false;
            if (simpleMovingAverage[currentDay - (int)daysInLookback] == 0) return false;

            //check that all values have the same sign for the length of the lookback
            var currentSign = Math.Sign(dailyStockPrices[currentDay].Close - simpleMovingAverage[currentDay]);

            for (int day = currentDay - (int)daysInLookback; day <= currentDay; day++)
            {
                //the lookback was zero so we can't tell trend.  This happens if we are too close to the start of data and no SMA can be calculated
                if (simpleMovingAverage[day] == 0)
                    return false;

                //this checks if all the values are either above or below the SMA
                if (Math.Sign(dailyStockPrices[day].Close - simpleMovingAverage[day]) != currentSign)
                    return false;   //If they all aren't the same sign, then they aren't all above/below the SMA, so we exit
            }

            //If we get here, we have met the Days In Lookback criteria and all the values are either above/below the current value
            if (currentSign > 0 && triggerUp)
            {
                //all values are trending above SMA and we are to trigger when up
                order = new Order()
                {
                    Price = dailyStockPrices[currentDay + 1].Open,
                    Reason = $"{daysInLookback} days above SMA",
                    TransactionDay = currentDay + 1,

                };

                transactionDay = order.TransactionDay;
                return true;
            }
            else if (currentSign < 0 && !triggerUp)
            {
                //all values are trending below the SMA and we are supposed to trigger when down

                order = new Order()
                {
                    Price = dailyStockPrices[currentDay + 1].Open,
                    Reason = $"{daysInLookback} days below SMA",
                    TransactionDay = currentDay + 1,

                };

                transactionDay = order.TransactionDay;
                return true;

            }

            //if we get here, no buy order was issued
            return false;
        }

        public AlgorithmDefinition GetDefaultDefinition(bool sellDefinition = false)
        {

            return new AlgorithmDefinition
            {
                SellDefinition = sellDefinition,
                Name = "SimpleMovingAverage",
                Parameters = new List<AlgorithmParameter>()
                    {
                        new AlgorithmParameter
                        {
                            Name ="DaysInMovingAverage",
                            NumericValue = 20,
                            UseRange = true,
                            RangeStart = 3,
                            RangeEnd = 30,
                            RangeStep = 1
                        },
                        new AlgorithmParameter
                        {
                            Name ="TriggerUp",
                            BooleanValue = !sellDefinition,
                            UseRange = true,
                            ValueIsBoolean=true,
                        },
                        new AlgorithmParameter
                        {
                            Name ="DaysInLookback",
                            NumericValue = 1,
                            UseRange = true,
                            RangeStart = 1,
                            RangeEnd = 10,
                            RangeStep = 1
                        },
                    }
            };

        }

        public void InitializeRun(RunAlgorithmDefinition definition)
        {
            daysInMovingAverage = definition.GetParameter("DaysInMovingAverage").NumericValue;

            daysInLookback = definition.GetParameter("DaysInLookback").NumericValue;

            triggerUp = definition.GetParameter("TriggerUp").BooleanValue;

        }

    }
}