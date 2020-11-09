using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StockTrader.Dtos;
using StockTrader.Data;

namespace StockTrader.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StocksController : ControllerBase
    {
        private readonly IStockDataRepo _stockDataRepo;
        private readonly IMapper _mapper;

        public StocksController(IStockDataRepo stockDataRepo, IMapper mapper)
        {
            _stockDataRepo = stockDataRepo;
            _mapper = mapper;
        }

        // GET: api/<StocksController>/{ticker}
        [HttpGet("{ticker}")]
        public ActionResult<IEnumerable<StockDataReadDto>> Get(string ticker)
        {
            var stocks = _stockDataRepo.Get(ticker).ToList();

            if (stocks.Count == 0) return NotFound();

            return Ok(_mapper.Map<List<StockDataReadDto>>(stocks));
        }

        // GET: api/<StocksController>/{ticker}/{startDate}/{endDate}
        [HttpGet("{ticker}/{startDate}/{endDate}")]
        public ActionResult<IEnumerable<StockDataReadDto>> Get(string ticker, DateTime startDate, DateTime endDate)
        {
            var stocks = _stockDataRepo.Get(ticker, startDate, endDate).ToList();

            if (stocks.Count == 0) return NotFound();

            return Ok(_mapper.Map<List<StockDataReadDto>>(stocks));
        }

        // GET: api/<StocksController>
        [HttpGet]
        public ActionResult<IEnumerable<StockReadDto>> Get()
        {
            var stocks = _stockDataRepo.GetAllStocks().ToList();

            if (!stocks.Any()) return NotFound();

            return Ok(_mapper.Map<List<StockReadDto>>(stocks));
        }

        // GET: api/<StocksController>/Query/{query}
        [HttpGet("Query/{query}")]
        public ActionResult<IEnumerable<StockReadDto>> GetQuery(string query)
        {
            var stocks = _stockDataRepo.QueryStocks(query).ToList();

            if (!stocks.Any()) return NotFound();

            return Ok(_mapper.Map<List<StockReadDto>>(stocks));
        }

    }
}