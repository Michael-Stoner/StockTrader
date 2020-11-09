using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace StockTrader.Data.Models
{
    public class Stock
    {
        [Key]
        public string Ticker { get; set; }

        [Required]
        public string Name { get; set; }
    }
}