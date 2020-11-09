using System;
using System.Collections.Generic;
using System.Text;

namespace StockTrader
{
    public class Order
    {
        public string Reason { get; set; }
        public double Price { get; set; }
        public int TransactionDay { get; set; }
    }
}