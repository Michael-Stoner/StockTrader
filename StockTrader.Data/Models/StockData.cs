using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace StockTrader.Data.Models
{
    //Stock Open High Low Close
    public class StockData
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(10)]
        public string Ticker { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [Column(TypeName = "decimal(14,4)")]
        public decimal Open { get; set; }

        [Required]
        [Column(TypeName = "decimal(14,4)")]
        public decimal High { get; set; }

        [Required]
        [Column(TypeName = "decimal(14,4)")]
        public decimal Close { get; set; }

        [Required]
        [Column(TypeName = "decimal(14,4)")]
        public decimal Low { get; set; }

        [Required]
        [Column(TypeName = "decimal(14,4)")]
        public decimal AdjustedClose { get; set; }

        [Required]
        public long Volume { get; set; }
    }
}