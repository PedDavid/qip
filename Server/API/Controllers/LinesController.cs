using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using API.Models;
using API.Models.Output;
using API.Models.Input;
using API.Models.IRepositories;
using API.Models.Extensions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers {
    [Route("api/boards/{boardId}/figures/[controller]")]
    public class LinesController : Controller {
        private readonly ILineRepository _lineRepository;

        public LinesController(ILineRepository lineRepository) {
            this._lineRepository = lineRepository;
        }

        [HttpGet]
        public IEnumerable<OutLine> GetAll(long boardId) {
            return _lineRepository
                .GetAll(boardId)
                .Select(LineExtensions.Out);
        }

        [HttpGet("{id}", Name = "GetLine")]
        public IActionResult GetById(long id, long boardId) {
            Line line = _lineRepository.Find(id, boardId);
            if(line == null) {
                return NotFound();
            }
            return new ObjectResult(line.Out());
        }

        [HttpPost]
        public IActionResult Create(long boardId, [FromBody] InLine inputLine) {
            if(inputLine == null || inputLine.BoardId != boardId || !inputLine.Id.HasValue) {
                return BadRequest();
            }

            Line line = new Line(boardId, inputLine.Id.Value).In(inputLine);
            long id = _lineRepository.Add(line);

            inputLine.Id = id;
            return CreatedAtRoute("GetLine", new { id = id, boardId = boardId }, inputLine);
        }


        [HttpPut("{id}")]
        public IActionResult Update(long id, long boardId, [FromBody] InLine inputLine) {
            if(inputLine == null || inputLine.Id != id || inputLine.BoardId != boardId) {
                return BadRequest();
            }

            Line line = _lineRepository.Find(id, boardId);
            if(line == null) {
                return NotFound();
            }

            line.In(inputLine);

            _lineRepository.Update(line);
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id, long boardId) {
            Line line = _lineRepository.Find(id, boardId);
            if(line == null) {
                return NotFound();
            }

            _lineRepository.Remove(id, boardId);
            return new NoContentResult();
        }
    }
}
