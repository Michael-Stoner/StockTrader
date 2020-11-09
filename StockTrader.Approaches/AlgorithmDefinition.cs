using StockTrader.Approaches.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace StockTrader.Approaches
{
    [Serializable]
    public class AlgorithmDefinition : AlgorithmBase
    {

        public List<AlgorithmParameter> Parameters { get; set; } = new List<AlgorithmParameter>();

        public AlgorithmParameter GetParameter(string name)
        {
            return Parameters.Find(p => p.Name == name);
        }

        [IgnoreDataMember]
        public string NameWithType
        {
            get
            {
                return $"{(SellDefinition ? "Sell:" : "Buy:")}{Name}";
            }
        }

    }
}