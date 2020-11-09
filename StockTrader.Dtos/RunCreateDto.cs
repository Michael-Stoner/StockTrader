using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StockTrader.Dtos
{
    public class RunCreateDto
    {
        [Required]
        public int ApproachId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Definition { get; set; }

    }
}