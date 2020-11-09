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
    public class RunsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IRunRepo _runRepo;

        public RunsController(IRunRepo runRepo, IMapper mapper)
        {
            _mapper = mapper;
            _runRepo = runRepo;
        }

        [HttpGet("{id}")]
        public ActionResult<RunReadDto> Get(int id)
        {
            var run = _runRepo.GetRun(id);

            if (run == null) return NotFound();

            return Ok(_mapper.Map<RunReadDto>(run));

        }

        [HttpGet("ByApproach/{approachId}")]
        public ActionResult<IEnumerable<RunReadDto>> GetRunsByApproach(int approachId)
        {
            var runs = _runRepo.GetRunsByApproach(approachId);

            if (!runs.Any()) return NotFound();

            return Ok(_mapper.Map<IEnumerable<RunReadDto>>(runs));

        }

        [HttpGet("{id}/definition")]
        public ActionResult<RunDefinitionReadDto> GetRunDefinition(int id)
        {
            var run = _runRepo.GetRun(id);

            if (run == null) return NotFound();

            return Ok(_mapper.Map<RunDefinitionReadDto>(run));
        }

        /// <summary>
        /// This method assigns Runs to the machine. This method is NOT IDEMPOTENT
        /// </summary>
        /// <param name="maxNumAssignments">Determines how many assignments can be made at one time.  This includes previously assigned runs that might not have been run yet.  This roughly corresponds to the max number of threads the machine can handle.</param>
        /// <param name="machineId">Unique identifier for the machine doing the work.  This must persist across calls and sessions.</param>
        /// <returns>A collection of Runs that are assigned to the machine</returns>
        [HttpPatch("MakeNewAssignments/{maxNumAssignments}/{machineId}")]
        public ActionResult<IEnumerable<RunReadDto>> GetNewAssignments(int maxNumAssignments, Guid machineId)
        {
            var runs = _runRepo.GetNewAssignments(maxNumAssignments, machineId);

            return Ok(_mapper.Map<IEnumerable<RunReadDto>>(runs));
        }

        [HttpPost]
        public ActionResult Create(RunCreateDto runCreateDto)
        {
            var run = _mapper.Map<Run>(runCreateDto);

            _runRepo.Create(run);
            _runRepo.SaveChanges();

            var readDto = _mapper.Map<RunReadDto>(run);

            return CreatedAtAction(nameof(Get), new { id = run.Id }, readDto);

        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var run = _runRepo.GetRun(id);

            if (run == null) return NotFound();

            _runRepo.Delete(run);
            _runRepo.SaveChanges();

            return NoContent();
        }

    }
}