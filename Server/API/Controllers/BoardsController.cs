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
    [Route("api/[controller]")]
    public class BoardsController : Controller {
        private readonly IBoardRepository _boardRepository;

        public BoardsController(IBoardRepository boardRepository) {
            this._boardRepository = boardRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<OutBoard>> GetAll(long index = 0, long size = 10) {
            IEnumerable<Board> boards = await _boardRepository.GetAllAsync(index,size);

            return boards.Select(BoardExtensions.Out);
        }

        [HttpGet("{id}", Name = "GetBoard")]
        public async Task<IActionResult> GetById(long id) {
            Board board = await _boardRepository.FindAsync(id);
            if(board == null) {
                return NotFound();
            }
            return new ObjectResult(board.Out());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InBoard inputBoard) {
            if(inputBoard == null) {
                return BadRequest();
            }

            Board board = new Board().In(inputBoard);
            long id = await _boardRepository.AddAsync(board);

            inputBoard.Id = id;
            return CreatedAtRoute("GetBoard", new { id = id }, inputBoard);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] InBoard inputBoard) {
            if(inputBoard == null || inputBoard.Id != id) {
                return BadRequest();
            }

            Board board = await _boardRepository.FindAsync(id);
            if(board == null) {
                return NotFound();
            }

            board.In(inputBoard);

            await _boardRepository.UpdateAsync(board);
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id) {
            Board board = await _boardRepository.FindAsync(id);
            if(board == null) {
                return NotFound();
            }

            await _boardRepository.RemoveAsync(id);
            return new NoContentResult();
        }
    }
}
