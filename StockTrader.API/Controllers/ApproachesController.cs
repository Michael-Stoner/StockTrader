using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StockTrader.Dtos;
using StockTrader.Data;
using StockTrader.Data.Models;

namespace StockTrader.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApproachesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IApproachRepo _approachRepo;

        public ApproachesController(IApproachRepo approachRepo, IMapper mapper)
        {
            _mapper = mapper;
            _approachRepo = approachRepo;
        }

        //GET api/<ApproachesController>
        [HttpGet]
        public ActionResult<IEnumerable<ApproachReadDto>> Get()
        {

            var approaches = _approachRepo.GetApproaches().ToList();

            if (approaches.Count == 0) return NotFound();

            return Ok(_mapper.Map<IEnumerable<ApproachReadDto>>(approaches));

        }

        //GET api/<ApproachesController>/{id}
        [HttpGet("{id}")]
        public ActionResult<IEnumerable<ApproachReadDto>> Get(int id)
        {

            var approach = _approachRepo.GetApproach(id);

            if (approach == null) return NotFound();

            return Ok(_mapper.Map<ApproachReadDto>(approach));

        }

        //DELETE api/<ApproachesController>/{id}
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var approach = _approachRepo.GetApproach(id);

            if (approach == null) return NotFound();

            _approachRepo.Delete(approach);

            _approachRepo.SaveChanges();

            return NoContent();
        }

        //POST api/<ApproachesController>
        [HttpPost]
        public ActionResult<ApproachReadDto> Create(ApproachCreateDto approachDto)
        {
            var approach = _mapper.Map<Approach>(approachDto);

            _approachRepo.Create(approach);
            _approachRepo.SaveChanges();

            var approachReadDto = _mapper.Map<ApproachReadDto>(approach);

            return CreatedAtAction(nameof(Get), new { id = approachReadDto.Id });

        }

    }
}