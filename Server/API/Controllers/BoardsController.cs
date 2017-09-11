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
    [Produces("application/json")]
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

        /// <summary>
        /// Returns a List of Boards
        /// </summary>
        /// <param name="search">Query for search by Boards</param>
        /// <param name="index">Number of the page</param>
        /// <param name="size">Number of Boards for page</param>
        /// <returns>Required List of Users</returns>
        /// <response code="200">Returns the required list of Boards</response>
        [HttpGet]
        [Authorize("Administrator")]
        [ProducesResponseType(typeof(IEnumerable<OutBoard>), 200)]
        public async Task<IEnumerable<OutBoard>> GetAllAsync(string search, long index = 0, long size = 10) {
            _logger.LogInformation(LoggingEvents.ListBoards, "Listing page {index} of Boards with size {size}", index, size);
            IEnumerable<Board> boards = await _boardService.GetAllAsync(index, size, search);

            return boards.Select(BoardExtensions.Out);
        }

        /// <summary>
        /// Returns a specific Board.
        /// </summary>
        /// <param name="id">Id of the Board to return</param>
        /// <returns>Required Board</returns>
        /// <response code="200">Returns the required Board</response>
        /// <response code="401">If the user needs authentication</response>
        /// <response code="403">If the user does not have authorization</response>
        /// <response code="404">If the Board not exists</response>
        [HttpGet("{id}", Name = "GetBoard")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(OutBoard), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetByIdAsync(long id) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(id), Policies.ReadBoardPolicy)) {
                _logger.LogWarning(LoggingEvents.GetBoardNotAuthorized, "GetByIdAsync({id}) NOT AUTHORIZED {user_id}", id, User.GetNameIdentifier());
                return Challenge();
            }

            _logger.LogInformation(LoggingEvents.GetBoard, "Getting Board {ID}", id);
            Board board = await _boardService.GetAsync(id);

            if(board == null) {
                //$"The Board with id {id} not exists"
                _logger.LogWarning(LoggingEvents.GetBoardNotFound, "GetByIdAsync({id}) NOT FOUND", id);
                return NotFound();
            }

            return Ok(board.Out());
        }

        /// <summary>
        /// Creates a Board.
        /// </summary>
        /// <remarks>
        /// The Authorization header is optional, but without that the Board is created fully public
        /// 
        /// Sample request:
        ///
        ///     POST /api/Board
        ///     Authorization: Bearer {ACCESS_TOKEN}
        /// 
        ///     {
        ///         "name": "Board Name",
        ///         "maxDistPoints": 10,
        ///         "basePermission": 0
        ///     }
        ///
        /// </remarks>
        /// <param name="inputBoard">Board's information</param>
        /// <returns>A newly-created Board</returns>
        /// <response code="201">Returns the newly-created Board</response>
        /// <response code="400">If the inputBoard is null</response> 
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(OutBoard), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] InCreateBoard inputBoard) {
            if(inputBoard == null) {
                _logger.LogDebug(LoggingEvents.InsertBoardWithoutBody, "Create() WITHOUT BODY");
                return BadRequest();
            }

            Board board = new Board().In(inputBoard);

            await _boardService.CreateAsync(board, User.GetNameIdentifier());
            _logger.LogInformation(LoggingEvents.InsertBoard, "Board {ID} Created", board.Id);

            return CreatedAtRoute("GetBoard", new { id = board.Id }, board.Out());
        }

        /// <summary>
        /// Updates a specific Board.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/Board/0
        ///     Authorization: Bearer {ACCESS_TOKEN}
        /// 
        ///     {
        ///         "id": 0,
        ///         "name": "Test Board Name",
        ///         "maxDistPoints": 15,
        ///         "basePermission": 1
        ///     }
        ///
        /// </remarks>
        /// <param name="id">Id of the Board to update</param>
        /// <param name="inputBoard">Information of the Board to update</param>
        /// <response code="204">Board update succeeds</response>
        /// <response code="400">If there is inconsistent information or the inputBoard is null</response>
        /// <response code="404">If the Board not exists</response>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(long id, [FromBody] InUpdateBoard inputBoard) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(id), Policies.BoardIsOwnPolicy)) {
                _logger.LogWarning(LoggingEvents.UpdateBoardNotAuthorized, "Update({ id}) NOT AUTHORIZED {user_id}", id, User.GetNameIdentifier());
                return Challenge();
            }

            if(inputBoard == null) {
                _logger.LogDebug(LoggingEvents.UpdateBoardWithoutBody, "Update({id}) WITHOUT BODY", id);
                return BadRequest();
            }

            if(inputBoard.Id != id) {
                //$"The id present on update is different of the expected. {Environment.NewLine}Expected: {id}{Environment.NewLine}Current: {inputBoard.Id}"
                _logger.LogDebug(LoggingEvents.UpdateBoardWrongId, "Update({ID}) WRONG ID {wrongId}", id, inputBoard.Id);
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

        /// <summary>
        /// Deletes a specific Board.
        /// </summary>
        /// <param name="id">Id of the Board to delete</param>
        /// <response code="204">Board deletion succeeds</response>
        /// <response code="404">If the Board not exists</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(long id) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(id), Policies.BoardIsOwnPolicy)) {
                _logger.LogWarning(LoggingEvents.DeleteBoardNotAuthorized, "Delete({id}) NOT AUTHORIZED {user_id}", id, User.GetNameIdentifier());
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
