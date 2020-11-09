using StockTrader.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockTrader.Data
{
    public interface IRunRepo
    {
        bool SaveChanges();

        bool SaveChanges(out int recordCount);

        void Create(Run run);

        void Update(Run run);

        void Delete(Run run);

        Run GetRun(int Id);

        IEnumerable<Run> GetRunsByApproach(int approachId);

        IEnumerable<Run> GetRuns();

        IEnumerable<Run> GetNewAssignments(int maxNumAssignments, Guid machineId);
    }
}