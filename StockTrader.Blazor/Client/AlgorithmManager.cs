using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using StockTrader.Approaches;

namespace StockTrader
{
    public static class AlgorithmManager
    {
        private static List<IApproachAlgorithm> allAlgorithmsCache { get; set; }
        private static List<IApproachAlgorithm> buyAlgorithmsCache { get; set; }
        private static List<IApproachAlgorithm> sellAlgorithmsCache { get; set; }

        public static List<IApproachAlgorithm> AllAlgorithms
        {
            get
            {
                if (allAlgorithmsCache != null) return allAlgorithmsCache;

                //If not cached, load approaches
                LoadApproachAlgorithms();

                return allAlgorithmsCache;

            }
        }

        public static List<IApproachAlgorithm> BuyAlgorithms
        {
            get
            {
                if (buyAlgorithmsCache != null) return buyAlgorithmsCache;

                //If not cached, load approaches
                LoadApproachAlgorithms();

                //filter approaches by buy types
                buyAlgorithmsCache = GetApproachesByType(false);

                return buyAlgorithmsCache;

            }
        }

        public static List<IApproachAlgorithm> SellAlgorithms
        {
            get
            {
                if (sellAlgorithmsCache != null) return sellAlgorithmsCache;

                //If not cached, load approaches
                LoadApproachAlgorithms();

                //filter approaches by sell types
                sellAlgorithmsCache = GetApproachesByType(true);

                return sellAlgorithmsCache;
            }
        }

        public static IApproachAlgorithm GetBuyAlagorithm(string name, bool deepCopy = false)
        {
            var algo = BuyAlgorithms.Where(a => a.GetDefaultDefinition().Name == name).FirstOrDefault();

            if (deepCopy)
            {
                //multiple threads could be running with the same algorithm,
                //so we deep copy it to ensure each one has a unique copy of the class
                return algo.DeepClone();
            }
            else
            {
                return algo;
            }

        }

        public static IApproachAlgorithm GetSellAlgorithm(string name, bool deepCopy = false)
        {
            var algo = SellAlgorithms.Where(a => a.GetDefaultDefinition().Name == name).FirstOrDefault();

            if (deepCopy)
            {
                //multiple threads could be running with the same algorithm,
                //so we deep copy it to ensure each one has a unique copy of the class
                return algo.DeepClone();
            }
            else
            {
                return algo;
            }
        }

        public static List<AlgorithmDefinition> GetAlgorithmDefaultDefinitions()
        {
            var definitions = GetAlgorithmDefaultDefinitions(true);

            definitions.AddRange(GetAlgorithmDefaultDefinitions(false));

            return definitions;

        }

        public static List<AlgorithmDefinition> GetAlgorithmDefaultDefinitions(bool sellDefinition)
        {
            List<AlgorithmDefinition> definitions = new List<AlgorithmDefinition>();

            var allAlgorithms = AllAlgorithms;  //forces cache to be built

            foreach (var algo in allAlgorithms)
            {
                var definiton = algo.GetDefaultDefinition(sellDefinition);

                if (definiton != null)
                {
                    definitions.Add(definiton);
                }
            }

            return definitions;
        }

        private static List<IApproachAlgorithm> GetApproachesByType(bool sellDefinition = false)
        {
            List<IApproachAlgorithm> list = new List<IApproachAlgorithm>();

            foreach (var approach in allAlgorithmsCache)
            {
                //check if the plugin has a buy approach
                if (approach.GetDefaultDefinition(sellDefinition) != null)
                {
                    list.Add(approach);
                }
            }

            return list;
        }

        private static void LoadApproachAlgorithms()
        {
            //TODO:  Lazy Load plugins

            //Until the lazy loading is complete, this will force the Plugins Assembly to load
            var plugin = new StocksTracker.Plugins.Buy.DaysOfRun();

            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .Where(x => typeof(IApproachAlgorithm).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .ToList();

            allAlgorithmsCache = new List<IApproachAlgorithm>();

            foreach (var algo in types)
            {
                allAlgorithmsCache.Add((IApproachAlgorithm)Activator.CreateInstance(algo));
            }

        }

    }
}