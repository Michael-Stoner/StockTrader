using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StockTrader.Dtos
{
    public class RunDefinitionReadDto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Definition { get; set; }

    }
}