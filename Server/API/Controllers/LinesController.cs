using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IODomain.Output;
using IODomain.Input;
using IODomain.Extensions;
using API.Interfaces.IRepositories;
using API.Domain;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers {
    [Route("api/boards/{boardId}/figures/[controller]")]
    public class LinesController : Controller {
        private readonly ILineRepository _lineRepository;

        public LinesController(ILineRepository lineRepository) {
            this._lineRepository = lineRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<OutLine>> GetAll(long boardId) {
            IEnumerable<Line> lines = await _lineRepository.GetAllAsync(boardId);

            return lines.Select(LineExtensions.Out);
        }

        [HttpGet("{id}", Name = "GetLine")]
        public async Task<IActionResult> GetById(long id, long boardId) {
            Line line = await _lineRepository.FindAsync(id, boardId);
            if(line == null) {
                return NotFound();
            }
            return new ObjectResult(line.Out());
        }

        [HttpPost]
        public async Task<IActionResult> Create(long boardId, [FromBody] InLine inputLine) {
            if(inputLine == null || inputLine.BoardId != boardId || !inputLine.Id.HasValue) {
                return BadRequest();
            }

            Line line = new Line(boardId, inputLine.Id.Value).In(inputLine);
            long id = await _lineRepository.AddAsync(line);

            inputLine.Id = id;
            return CreatedAtRoute("GetLine", new { id = id, boardId = boardId }, inputLine);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, long boardId, [FromBody] InLine inputLine) {
            if(inputLine == null || inputLine.Id != id || inputLine.BoardId != boardId) {
                return BadRequest();
            }

            Line line = await _lineRepository.FindAsync(id, boardId);
            if(line == null) {
                return NotFound();
            }

            line.In(inputLine);

            await _lineRepository.UpdateAsync(line);
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id, long boardId) {
            Line line = await _lineRepository.FindAsync(id, boardId);
            if(line == null) {
                return NotFound();
            }

            await _lineRepository.RemoveAsync(id, boardId);
            return new NoContentResult();
        }
    }
}
