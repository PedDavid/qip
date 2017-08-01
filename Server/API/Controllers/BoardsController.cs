using API.Filters;
using API.Interfaces.IServices;
using IODomain.Input;
using IODomain.Output;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers {
    [ServicesExceptionFilter]
    [Route("api/[controller]")]
    public class BoardsController : Controller {
        private readonly IBoardService _boardService;

        public BoardsController(IBoardService boardService) {
            _boardService = boardService;
        }

        [HttpGet]
        public Task<IEnumerable<OutBoard>> GetAllAsync(string search, long index = 0, long size = 10) {
            return _boardService.GetAllAsync(index, size, search);
        }

        [HttpGet("{id}", Name = "GetBoard")]
        public Task<OutBoard> GetByIdAsync(long id) {
            return _boardService.GetAsync(id);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InBoard inputBoard) {
            OutBoard board = await _boardService.CreateAsync(inputBoard);
            return CreatedAtRoute("GetBoard", new { id = board.Id }, board);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] InBoard inputBoard) {
            await _boardService.UpdateAsync(id, inputBoard);
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id) {
            await _boardService.DeleteAsync(id);
            return new NoContentResult();
        }
    }
}
