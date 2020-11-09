using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace StockTrader.API
{
    public class CorsPolicyConfiguration
    {
        public string DefaultPolicyName { get; set; }
        public List<CorsPolicyItem> Policies { get; set; }

        public class CorsPolicyItem
        {
            public string PolicyName { get; set; }
            public string[] Origins { get; set; }

        }
    }
}