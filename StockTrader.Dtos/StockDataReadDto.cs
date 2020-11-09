using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StockTrader.Dtos
{
    public class StockDataReadDto
    {

        [Key]
        public int Id { get; set; }

        [Required]
        public string Ticker { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public decimal Open { get; set; }

        [Required]
        public decimal High { get; set; }

        [Required]
        public decimal Close { get; set; }

        [Required]
        public decimal Low { get; set; }

        [Required]
        public long Volume { get; set; }

    }
}