using StockTrader.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace StockTrader.Data.Models
{
    public class Run
    {
        [Key]
        public int Id { get; set; }

        public int ApproachId { get; set; }

        [Required]
        public Approach Approach { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Definition { get; set; }

        [Required]
        public RunStatus Status { get; set; } = RunStatus.Pending;

        public Guid MachineId { get; set; }

        public Guid AssignedKey { get; set; }

        public DateTime AssignedDate { get; set; }

    }
}