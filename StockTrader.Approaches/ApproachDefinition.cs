using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace StockTrader.Approaches
{
    [Serializable]
    public class ApproachDefinition
    {
        public int Id { get; set; }

        public List<string> StockTickers { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public double StartingDollars { get; set; } = 10000;

        public double DiscardThreshold { get; set; } = 0.17;

        public List<AlgorithmDefinition> BuyAlgorithms { get; set; } = new List<AlgorithmDefinition>();
        public List<AlgorithmDefinition> SellAlgorithms { get; set; } = new List<AlgorithmDefinition>();

        [IgnoreDataMember]
        public string Name
        {
            get
            {
                string buyDescriptions = Common.GetAlgorithmsDescriptions(BuyAlgorithms.OrderBy(a => a.Priority));
                string sellDescriptions = Common.GetAlgorithmsDescriptions(SellAlgorithms.OrderBy(a => a.Priority));

                return $"B{buyDescriptions} S{sellDescriptions}";
            }
        }

        [IgnoreDataMember]
        public List<AlgorithmDefinition> AllAlgorithms
        {
            get
            {
                var list = new List<AlgorithmDefinition>();
                list.AddRange(BuyAlgorithms);
                list.AddRange(SellAlgorithms);

                return list;
            }
        }
    }
}