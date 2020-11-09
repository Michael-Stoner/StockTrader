using StockTrader.Approaches.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockTrader.Approaches
{
    [Serializable]
    public class AlgorithmParameter : AlgorithmParameterBase
    {
        public bool UseRange { get; set; }
        public double RangeStart { get; set; }
        public double RangeEnd { get; set; }
        public double RangeStep { get; set; }

        public void Initialize()
        {
            if (UseRange)
            {
                _value = RangeStart;
            }

        }
    }

}