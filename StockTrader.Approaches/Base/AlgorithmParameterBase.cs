using System;
using System.Collections.Generic;
using System.Text;

namespace StockTrader.Approaches.Base
{
    [Serializable]
    public class AlgorithmParameterBase
    {
        public string Name { get; set; }

        protected object _value { get; set; }
        public bool ValueIsBoolean { get; set; }

        public bool BooleanValue
        {
            get
            {
                if (!ValueIsBoolean) return false;
                if (_value == null) return false;

                return (bool)(_value ?? false);
            }
            set
            {
                _value = value;
            }
        }

        public double NumericValue
        {
            get
            {
                if (_value is double)
                    return (double)(_value ?? 0);
                else
                    return 0;
            }
            set
            {
                _value = value;
            }
        }
    }
}