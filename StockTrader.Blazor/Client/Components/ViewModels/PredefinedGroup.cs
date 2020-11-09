using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockTrader.Blazor.Client.Components.ViewModels
{
    public class PredefinedGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> StockTickers { get; set; }

    }
}