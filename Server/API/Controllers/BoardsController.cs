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
        public IEnumerable<OutBoard> GetAll() {
            return _boardRepository
                .GetAll()
                .Select(BoardExtensions.Out);
        }

        [HttpGet("{id}", Name = "GetBoard")]
        public IActionResult GetById(long id) {
            Board board = _boardRepository.Find(id);
            if(board == null) {
                return NotFound();
            }
            return new ObjectResult(board.Out());
        }

        [HttpPost]
        public IActionResult Create([FromBody] InBoard inputBoard) {
            if(inputBoard == null) {
                return BadRequest();
            }

            Board board = new Board().In(inputBoard);
            long id = _boardRepository.Add(board);

            inputBoard.Id = id;
            return CreatedAtRoute("GetBoard", new { id = id }, inputBoard);
        }

        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] InBoard inputBoard) {
            if(inputBoard == null || inputBoard.Id != id) {
                return BadRequest();
            }

            Board board = _boardRepository.Find(id);
            if(board == null) {
                return NotFound();
            }

            board.In(inputBoard);

            _boardRepository.Update(board);
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id) {
            Board board = _boardRepository.Find(id);
            if(board == null) {
                return NotFound();
            }

            _boardRepository.Remove(id);
            return new NoContentResult();
        }
    }
}
