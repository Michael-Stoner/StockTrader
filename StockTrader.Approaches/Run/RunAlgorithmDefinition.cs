using StockTrader.Approaches.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StockTrader.Approaches.Run
{
    public class RunAlgorithmDefinition : AlgorithmBase
    {

        public List<AlgorithmParameterBase> Parameters { get; set; }

        public AlgorithmParameterBase GetParameter(string name)
        {
            return Parameters.FirstOrDefault(p => p.Name == name);
        }

    }
}