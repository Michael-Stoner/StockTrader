using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace StockTrader.Approaches
{
    public static class AlgorithmManager
    {
        private static List<IApproachAlgorithm> AllAlgorithmsCache { get; set; }
        private static List<IApproachAlgorithm> BuyAlgorithmsCache { get; set; }
        private static List<IApproachAlgorithm> SellAlgorithmsCache { get; set; }

        public static List<IApproachAlgorithm> AllAlgorithms
        {
            get
            {
                if (AllAlgorithmsCache != null) return AllAlgorithmsCache;

                //If not cached, load approaches
                LoadApproachAlgorithms();

                return AllAlgorithmsCache;
            }
        }

        public static List<IApproachAlgorithm> BuyAlgorithms
        {
            get
            {
                if (BuyAlgorithmsCache != null) return BuyAlgorithmsCache;

                //If not cached, load approaches
                LoadApproachAlgorithms();

                //filter approaches by buy types
                BuyAlgorithmsCache = GetApproachesByType(false);

                return BuyAlgorithmsCache;
            }
        }

        public static List<IApproachAlgorithm> SellAlgorithms
        {
            get
            {
                if (SellAlgorithmsCache != null) return SellAlgorithmsCache;

                //If not cached, load approaches
                LoadApproachAlgorithms();

                //filter approaches by sell types
                SellAlgorithmsCache = GetApproachesByType(true);

                return SellAlgorithmsCache;
            }
        }

        public static IApproachAlgorithm GetBuyAlagorithm(string name, bool deepCopy = false)
        {
            var algo = BuyAlgorithms.Find(a => a.GetDefaultDefinition().Name == name);

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
            var algo = SellAlgorithms.Find(a => a.GetDefaultDefinition().Name == name);

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

            foreach (var approach in AllAlgorithmsCache)
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
            //var plugin = new SimpleMovingAverage();

            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .Where(x => typeof(IApproachAlgorithm).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .ToList();

            AllAlgorithmsCache = new List<IApproachAlgorithm>();

            foreach (var algo in types)
            {
                AllAlgorithmsCache.Add((IApproachAlgorithm)Activator.CreateInstance(algo));
            }
        }
    }
}