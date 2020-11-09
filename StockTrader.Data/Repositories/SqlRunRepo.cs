using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using StockTrader.Data.Models;
using StockTrader.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StockTrader.Data.Repositories
{
    public class SqlRunRepo : IRunRepo
    {
        private readonly StockTraderContext _stockTraderContext;

        public SqlRunRepo(StockTraderContext stockTraderContext)
        {
            _stockTraderContext = stockTraderContext;
        }

        public bool SaveChanges(out int recordCount)
        {
            recordCount = _stockTraderContext.SaveChanges();
            return (recordCount >= 0);
        }

        public bool SaveChanges()
        {
            return (_stockTraderContext.SaveChanges() >= 0);
        }

        public void Create(Run run)
        {
            _stockTraderContext.Runs.Add(run);
        }

        public void Delete(Run run)
        {
            _stockTraderContext.Runs.Remove(run);
        }

        public Run GetRun(int id)
        {
            return _stockTraderContext.Runs.FirstOrDefault(r => r.Id.Equals(id));
        }

        public IEnumerable<Run> GetRuns()
        {
            return _stockTraderContext.Runs.ToList();
        }

        public void Update(Run run)
        {

        }

        public IEnumerable<Run> GetRunsByApproach(int approachId)
        {
            return _stockTraderContext.Runs.Where(r => r.ApproachId.Equals(approachId)).ToList();
        }

        public IEnumerable<Run> GetNewAssignments(int maxNumAssignments, Guid machineId)
        {
            //get current assignments and add assignments up to max number
            var numToAssign = maxNumAssignments -
                        (_stockTraderContext.Runs
                                .Where(r => r.Status == RunStatus.Assigned
                                        && r.MachineId.Equals(machineId))
                                .Count());

            if (numToAssign > 0)
            {

                var assignedKey = Guid.NewGuid();

                AssignRunsToMachine(numToAssign, assignedKey, machineId);
            }

            return _stockTraderContext.Runs
                                .Where(r => r.Status == RunStatus.Assigned
                                        && r.MachineId.Equals(machineId))
                                .ToList();

        }

        private void AssignRunsToMachine(int numToAssign, Guid assignedKey, Guid machineId)
        {
            var parameters = new[] {
                        new SqlParameter("@Rows", numToAssign),
                        new SqlParameter("@AssignedKey", assignedKey),
                        new SqlParameter("@AssignedDate", DateTime.Now),
                        new SqlParameter("@MachineId", machineId),
                        new SqlParameter("@PendingStatus", RunStatus.Pending),
                        new SqlParameter("@NewStatus", RunStatus.Assigned),
                };

            var sql = "UPDATE Runs SET " +
                            "AssignedKey = @AssignedKey," +
                            "AssignedDate = @AssignedDate, " +
                            "MachineId = @MachineId, " +
                            "Status = @NewStatus " +
                            "WHERE ID IN " +
                            $"   (SELECT TOP {numToAssign} ID FROM Runs WHERE Status=@PendingStatus ORDER BY Id)";

            _stockTraderContext.Database.ExecuteSqlRaw(sql, parameters);
        }
    }
}