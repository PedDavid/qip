using API.Filters;
using API.Interfaces.IServices;
using IODomain.Input;
using IODomain.Output;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers {
    [ServicesExceptionFilter]
    [Route("api/boards/{boardId}/figures/[controller]")]
    public class LinesController : Controller {
        private readonly ILineService _lineService;


        public LinesController(ILineService lineService) {
            _lineService = lineService;
        }

        [HttpGet]
        public Task<IEnumerable<OutLine>> GetAll(long boardId) {
            return _lineService.GetAllAsync(boardId);
        }

        [HttpGet("{id}", Name = "GetLine")]
        public Task<OutLine> GetById(long id, long boardId) {
            return _lineService.GetAsync(id, boardId);
        }

        [HttpPost]
        public async Task<IActionResult> Create(long boardId, [FromBody] InLine inputLine) {
            OutLine line = await _lineService.CreateAsync(boardId, inputLine);
            return CreatedAtRoute("GetLine", new { id = line.Id, boardId = boardId }, line);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, long boardId, [FromBody] InLine inputLine) {
            await _lineService.UpdateAsync(id, boardId, inputLine);
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id, long boardId) {
            await _lineService.DeleteAsync(id, boardId);
            return new NoContentResult();
        }
    }
}
