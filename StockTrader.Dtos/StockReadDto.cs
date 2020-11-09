using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace StockTrader.Dtos
{
    public class StockReadDto
    {
        [Key]
        public string Ticker { get; set; }

        [Required]
        public string Name { get; set; }

    }
}