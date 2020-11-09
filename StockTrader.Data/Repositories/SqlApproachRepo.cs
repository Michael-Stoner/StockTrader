using StockTrader.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StockTrader.Data.Repositories
{
    public class SqlApproachRepo : IApproachRepo
    {
        private readonly StockTraderContext _stockTraderContext;

        public SqlApproachRepo(StockTraderContext stockTraderContext)
        {
            _stockTraderContext = stockTraderContext;

        }

        public void Create(Approach approach)
        {
            if (approach == null) throw new ArgumentNullException(nameof(approach));

            _stockTraderContext.Approaches.Add(approach);
        }

        public void Delete(Approach approach)
        {
            if (approach == null) throw new ArgumentNullException(nameof(approach));

            _stockTraderContext.Approaches.Remove(approach);
        }

        public Approach GetApproach(int id)
        {
            return _stockTraderContext.Approaches.FirstOrDefault(a => a.Id.Equals(id));
        }

        public IEnumerable<Approach> GetApproaches()
        {
            return _stockTraderContext.Approaches.ToList();
        }

        public bool SaveChanges()
        {
            return (_stockTraderContext.SaveChanges() >= 0);
        }

        public bool SaveChanges(out int recordCount)
        {
            recordCount = _stockTraderContext.SaveChanges();
            return (recordCount >= 0);
        }

        public void Update(Approach approach)
        {
            throw new NotImplementedException();
        }
    }
}