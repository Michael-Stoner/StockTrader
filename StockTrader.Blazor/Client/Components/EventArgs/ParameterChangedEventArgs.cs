using StockTrader.Approaches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockTrader.Blazor.Client.Components.EventArgs
{
    public class ParameterChangedEventArgs
    {
        public AlgorithmDefinition AlgorithmDefinition { get; set; }
        public AlgorithmParameter AlgorithmParameter { get; set; }

    }
}