using CsvHelper.Configuration.Attributes;
using StockTrader.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockTrader.Data
{
    public interface IApproachRepo
    {
        bool SaveChanges();

        bool SaveChanges(out int recordCount);

        void Create(Approach approach);

        void Update(Approach approach);

        void Delete(Approach approach);

        Approach GetApproach(int id);

        IEnumerable<Approach> GetApproaches();

    }
}