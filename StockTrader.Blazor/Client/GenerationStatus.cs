using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace StockTrader.Blazor.Client
{
    public class GenerationStatus
    {
        public int ApproachId { get; set; }
        public int TotalPermutations { get; set; }
        public int CurrentPermutation { get; set; }
        public int SavedPermutations { get; set; }

        public static string GetKey(int approachId)
        {
            return $"ApproachStatus:{approachId}";
        }

    }

    public static class GenerationStatusExtensions
    {
        public static string GetKey(this GenerationStatus status)
        {
            return GenerationStatus.GetKey(status.ApproachId);
        }
    }
}