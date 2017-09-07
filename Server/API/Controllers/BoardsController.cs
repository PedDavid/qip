﻿using API.Domain;
using API.Filters;
using API.Interfaces.IServices;
using Authorization;
using Authorization.Extensions;
using Authorization.Resources;
using IODomain.Extensions;
using IODomain.Input;
using IODomain.Output;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers {
    [Route("api/[controller]")]
    [ValidateModel]
    [ServicesExceptionFilter]
    public class BoardsController : Controller {
        private readonly IBoardService _boardService;
        private readonly IAuthorizationService _authorizationService;

        public BoardsController(IBoardService boardService, IAuthorizationService authorizationService) {
            _boardService = boardService;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        [Authorize("Administrator")]
        public async Task<IEnumerable<OutBoard>> GetAllAsync(string search, long index = 0, long size = 10) {
            IEnumerable<Board> boards = await _boardService.GetAllAsync(index, size, search);

            return boards.Select(BoardExtensions.Out);
        }

        [HttpGet("{id}", Name = "GetBoard")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(OutBoard),200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetByIdAsync(long id) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(id), Policies.ReadBoardPolicy))
                return Challenge();

            Board board = await _boardService.GetAsync(id);

            if(board == null) {
                //$"The Board with id {id} not exists"
                return NotFound();
            }

            return Ok(board.Out());
        }

        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(OutBoard), 201)]
        public async Task<IActionResult> Create([FromBody] InCreateBoard inputBoard) {
            Board board = new Board().In(inputBoard);

            await _boardService.CreateAsync(board, User.GetNameIdentifier());

            return CreatedAtRoute("GetBoard", new { id = board.Id }, board.Out());
        }

        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(long id, [FromBody] InUpdateBoard inputBoard) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(id), Policies.BoardIsOwnPolicy))
                return Challenge();

            if(inputBoard.Id != id) {
                //$"The id present on update is different of the expected. {Environment.NewLine}Expected: {id}{Environment.NewLine}Current: {inputBoard.Id}"
                return BadRequest();
            }

            Board board = await _boardService.GetAsync(id);
            if(board == null) {
                //$"The Board with id {id} not exists"
                return NotFound();
            }

            board.In(inputBoard);

            await _boardService.UpdateAsync(board);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(long id) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(id), Policies.BoardIsOwnPolicy))
                return Challenge();

            if(!await _boardService.ExistsAsync(id)) {
                // $"The Board with id {id} not exists"
                return NotFound();
            }

            await _boardService.DeleteAsync(id);

            return NoContent();
        }
    }
}
