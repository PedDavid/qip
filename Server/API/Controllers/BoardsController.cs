using API.Domain;
using API.Filters;
using API.Interfaces;
using API.Interfaces.IServices;
using Authorization;
using Authorization.Extensions;
using Authorization.Resources;
using IODomain.Extensions;
using IODomain.Input;
using IODomain.Output;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<BoardsController> _logger;

        public BoardsController(IBoardService boardService, IAuthorizationService authorizationService, ILogger<BoardsController> logger) {
            _boardService = boardService;
            _authorizationService = authorizationService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize("Administrator")]
        public async Task<IEnumerable<OutBoard>> GetAllAsync(string search, long index = 0, long size = 10) {
            _logger.LogInformation(LoggingEvents.ListBoards, "Listing all Boards");
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
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(id), Policies.ReadBoardPolicy)) {
                _logger.LogWarning(LoggingEvents.UpdateBoardNotAuthorized, "GetByIdAsync({ id}) NOT AUTHORIZED {userId}", id, User.GetNameIdentifier());
                return Challenge();
            }

            _logger.LogInformation(LoggingEvents.GetBoard, "Getting item {ID}", id);
            Board board = await _boardService.GetAsync(id);

            if(board == null) {
                //$"The Board with id {id} not exists"
                _logger.LogWarning(LoggingEvents.GetBoardNotFound,"GetByIdAsync({id}) NOT FOUND", id);
                return NotFound();
            }

            return Ok(board.Out());
        }

        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(OutBoard), 201)]
        public async Task<IActionResult> Create([FromBody] InCreateBoard inputBoard) {
            if(inputBoard == null) {
                _logger.LogDebug(LoggingEvents.CreateWithoutBody, "Create() WITHOUT BODY");
                return BadRequest();
            }

            Board board = new Board().In(inputBoard);

            await _boardService.CreateAsync(board, User.GetNameIdentifier());
            _logger.LogInformation(LoggingEvents.InsertBoard, "Board {ID} Created", board.Id);

            return CreatedAtRoute("GetBoard", new { id = board.Id }, board.Out());
        }

        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(long id, [FromBody] InUpdateBoard inputBoard) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(id), Policies.BoardIsOwnPolicy)) {
                _logger.LogWarning(LoggingEvents.UpdateBoardNotAuthorized, "Update({ id}) NOT AUTHORIZED {userId}", id, User.GetNameIdentifier());
                return Challenge();
            }

            if(inputBoard == null) {
                _logger.LogDebug(LoggingEvents.UpdateWithoutBody, "Update() WITHOUT BODY");
                return BadRequest();
            }

            if(inputBoard.Id != id) {
                //$"The id present on update is different of the expected. {Environment.NewLine}Expected: {id}{Environment.NewLine}Current: {inputBoard.Id}"
                _logger.LogDebug(LoggingEvents.UpdateBoardWrongId, "Update({ID}) WRONG ID {wrongId}", id);
                return BadRequest();
            }

            Board board = await _boardService.GetAsync(id);
            if(board == null) {
                //$"The Board with id {id} not exists"
                _logger.LogWarning(LoggingEvents.UpdateBoardNotFound, "Update({ID}) NOT FOUND", id);
                return NotFound();
            }

            board.In(inputBoard);

            await _boardService.UpdateAsync(board);
            _logger.LogInformation(LoggingEvents.UpdateBoard, "Board {ID} Updated", board.Id);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(long id) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(id), Policies.BoardIsOwnPolicy)) {
                _logger.LogWarning(LoggingEvents.UpdateBoardNotAuthorized, "Delete({ id}) NOT AUTHORIZED {userId}", id, User.GetNameIdentifier());
                return Challenge();
            }

            if(!await _boardService.ExistsAsync(id)) {
                // $"The Board with id {id} not exists"
                _logger.LogWarning(LoggingEvents.DeleteBoardNotFound, "Delete({ID}) NOT FOUND", id);
                return NotFound();
            }

            await _boardService.DeleteAsync(id);
            _logger.LogInformation(LoggingEvents.DeleteBoard, "Board {ID} Deleted", id);

            return NoContent();
        }
    }
}
