using System;
using System.Collections.Generic;
using System.Text;

namespace StockTrader.Approaches.Base
{
    [Serializable]
    public class AlgorithmBase
    {
        public Guid Id { get; set; }

        public bool SellDefinition { get; set; } = true;

        public string Name { get; set; }
        public string Description { get; set; }

        public int Priority { get; set; }
    }
}