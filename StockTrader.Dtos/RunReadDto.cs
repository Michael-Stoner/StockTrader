using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StockTrader.Dtos
{
    public class RunReadDto
    {
        //[Key]
        public int Id { get; set; }

        //[Required]
        public int ApproachId { get; set; }

        //[Required]
        public string Name { get; set; }

        //[Required]
        public RunStatus Status { get; set; }

    }

    public enum RunStatus
    {
        Unavailable,
        Pending,
        Assigned,
        Running,
        Finished,
        Discarded
    }
}