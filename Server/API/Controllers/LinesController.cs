using QIP.Domain;
using QIP.API.Filters;
using QIP.Public;
using QIP.Public.IServices;
using Authorization;
using Authorization.Extensions;
using Authorization.Resources;
using QIP.IODomain.Extensions;
using QIP.IODomain.Input;
using QIP.IODomain.Output;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QIP.API.Controllers {
    [ValidateModel]
    [ServicesExceptionFilter]
    [Produces("application/json")]
    [Route("api/boards/{boardId}/figures/[controller]")]
    public class LinesController : Controller {
        private readonly ILineService _lineService;
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger<LinesController> _logger;

        public LinesController(ILineService lineService, IAuthorizationService authorizationService, ILogger<LinesController> logger) {
            _lineService = lineService;
            _authorizationService = authorizationService;
            _logger = logger;
        }

        /// <summary>
        /// Returns a List of Lines
        /// </summary>
        /// <param name="boardId">Id of the Board to which the Lines belong</param>
        /// <returns>Required List of Lines</returns>
        /// <response code="200">Returns the required list of Lines</response>
        /// <response code="401">If the user needs authentication</response>
        /// <response code="403">If the user does not have authorization</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<OutLine>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetAll(long boardId) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.ReadBoardPolicy)) {
                _logger.LogWarning(LoggingEvents.ListLinesNotAuthorized, "GetAll({boardId}) NOT AUTHORIZED {user_id}", boardId, User.GetNameIdentifier());
                return Challenge();
            }

            _logger.LogInformation(LoggingEvents.ListLines, "Listing all Lines");
            IEnumerable<Line> lines = await _lineService.GetAllAsync(boardId);

            return Ok(lines.Select(LineExtensions.Out));
        }

        /// <summary>
        /// Returns a specific Line
        /// </summary>
        /// <param name="id">Id of the Line to return</param>
        /// <param name="boardId">Id of the Board to which the Line belong</param>
        /// <returns>Required Line</returns>
        /// <response code="200">Returns the required Line</response>
        /// <response code="401">If the user needs authentication</response>
        /// <response code="403">If the user does not have authorization</response>
        /// <response code="404">if the Line does not exist in the Board</response>
        [HttpGet("{id}", Name = "GetLine")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(OutLine), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(long id, long boardId) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.ReadBoardPolicy)) {
                _logger.LogWarning(LoggingEvents.GetLineNotAuthorized, "GetById({id}, {boardId}) NOT AUTHORIZED {user_id}", id, boardId, User.GetNameIdentifier());
                return Challenge();
            }

            _logger.LogInformation(LoggingEvents.GetLine, "Getting Line {id}", id);
            Line line = await _lineService.GetAsync(id, boardId);

            if(line == null) {
                //$"The line with id {id}, belonging to board with id {boardId}, does not exist"
                _logger.LogWarning(LoggingEvents.GetLineNotFound, "GetById({id}, {boardId}) NOT FOUND", id, boardId);
                return NotFound();
            }

            return Ok(line.Out());
        }

        /// <summary>
        /// Creates a Line in the specific Board
        /// </summary>
        /// <remarks>
        /// The Authorization header is optional in some cases
        /// 
        /// Sample request:
        ///
        ///     POST /api/board/0/figures/lines
        ///     Content-Type: application/json
        ///     Authorization: Bearer {ACCESS_TOKEN}
        /// 
        ///     {
        ///         "boardId": 0,
        ///         "style": { "color": "red" },
        ///         "closed": "false",
        ///         "points": [
        ///             {
        ///                 "x": 1, 
        ///                 "y": 1, 
        ///                 "idx": 0, 
        ///                 "style": {"width": 3}
        ///             },
        ///             {
        ///                 "x": 3, 
        ///                 "y": 3, 
        ///                 "idx": 1, 
        ///                 "style": {"width": 3}
        ///             }	
        ///         ]
        ///     }
        ///
        /// </remarks>
        /// <param name="boardId">Id of the board to which the Line should belong</param>
        /// <param name="inLine">Information of the Line to create</param>
        /// <returns>A newly-created Line</returns>
        /// <response code="201">Returns the newly-created Line</response>
        /// <response code="400">If there is inconsistent information or the inLine is null</response> 
        /// <response code="401">If the user needs authentication</response>
        /// <response code="403">If the user does not have authorization</response>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(OutLine), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> Create(long boardId, [FromBody] InCreateLine inLine) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.WriteBoadPolicy)) {
                _logger.LogWarning(LoggingEvents.InsertLineNotAuthorized, "Create({boardId}) NOT AUTHORIZED {user_id}", boardId, User.GetNameIdentifier());
                return Challenge();
            }

            if(inLine == null) {
                _logger.LogDebug(LoggingEvents.InsertLineWithoutBody, "Create({boardId}) WITHOUT BODY", boardId);
                return BadRequest();
            }

            if(inLine.BoardId != boardId) {
                //$"The board id present on update is different of the expected. {Environment.NewLine}Expected: {boardId}{Environment.NewLine}Current: {inLine.BoardId}"
                _logger.LogDebug(LoggingEvents.InsertLineWrongBoardId, "Create({boardId}) WRONG BOARD ID {wrongBoardId}", boardId, inLine.BoardId);
                return BadRequest();
            }

            Line line = new Line().In(inLine);

            await _lineService.CreateAsync(line);
            _logger.LogInformation(LoggingEvents.InsertLine, "Line {id} of Board {boardId} Created", line.Id, boardId);

            return CreatedAtRoute("GetLine", new { id = line.Id, boardId = boardId }, line.Out());
        }

        /// <summary>
        /// Updates a specific Line
        /// </summary>
        /// <remarks>
        /// The Authorization header is optional in some cases
        /// 
        /// Sample request:
        ///
        ///     PUT /api/board/0/figures/lines/0
        ///     Content-Type: application/json
        ///     Authorization: Bearer {ACCESS_TOKEN}
        /// 
        ///     {
        ///         "id": 0,
        ///         "boardId": 0,
        ///         "style": { "color": "blue" },
        ///         "closed": "true",
        ///         "points": [
        ///             {
        ///                 "x": 9, 
        ///                 "y": 0, 
        ///                 "idx": 0, 
        ///                 "style": {"width": 2}
        ///             },
        ///             {
        ///                 "x": 1, 
        ///                 "y": 8, 
        ///                 "idx": 1, 
        ///                 "style": {"width": 1}
        ///             }	
        ///         ]
        ///     }
        ///
        /// </remarks>
        /// <param name="id">Id of the Line to update</param>
        /// <param name="boardId">Id of the Board to which the Line belong</param>
        /// <param name="inLine">Information of the Line to update</param>
        /// <response code="204">Line update succeeds</response>
        /// <response code="400">If there is inconsistent information or the inLine is null</response>
        /// <response code="401">If the user needs authentication</response>
        /// <response code="403">If the user does not have authorization</response>
        /// <response code="404">if the Line does not exist in the Board</response>
        [HttpPut("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(long id, long boardId, [FromBody] InUpdateLine inLine) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.WriteBoadPolicy)) {
                _logger.LogWarning(LoggingEvents.UpdateLineNotAuthorized, "Update({id}, {boardId}) NOT AUTHORIZED {user_id}", id, boardId, User.GetNameIdentifier());
                return Challenge();
            }

            if(inLine == null) {
                _logger.LogDebug(LoggingEvents.UpdateLineWithoutBody, "Update({id}, {boardId}) WITHOUT BODY", id, boardId);
                return BadRequest();
            }

            if(inLine.Id != id) {
                //$"The id present on update is different of the expected. {Environment.NewLine}Expected: {id}{Environment.NewLine}Current: {inLine.Id}"
                _logger.LogDebug(LoggingEvents.UpdateLineWrongBoardId, "Update({id}, {boardId}) WRONG ID {wrongId}", id, boardId, inLine.Id);
                return BadRequest();
            }

            if(inLine.BoardId != boardId) {
                //$"The board id present on update is different of the expected. {Environment.NewLine}Expected: {boardId}{Environment.NewLine}Current: {inLine.BoardId}"
                _logger.LogDebug(LoggingEvents.UpdateLineWrongBoardId, "Update({id}, {boardId}) WRONG BOARD ID {wrongBoardId}", id, boardId, inLine.BoardId);
                return BadRequest();
            }

            Line line = await _lineService.GetAsync(id, boardId);
            if(line == null) {
                //$"The line with id {id}, belonging to board with id {boardId}, does not exist"
                _logger.LogWarning(LoggingEvents.UpdateLineNotFound, "Update({id}, {boardId}) NOT FOUND", id, boardId);
                return NotFound();
            }

            line.In(inLine);

            await _lineService.UpdateAsync(line);
            _logger.LogInformation(LoggingEvents.UpdateLine, "Line {id} of Board {boardId} Updated", id, boardId);


            return NoContent();
        }

        /// <summary>
        /// Deletes a specific Line
        /// </summary>
        /// <param name="id">Id of the Line to delete</param>
        /// <param name="boardId">Id of the Board to which the Line belong</param>
        /// <response code="204">Line deletion succeeds</response>
        /// <response code="401">If the user needs authentication</response>
        /// <response code="403">If the user does not have authorization</response>
        /// <response code="404">if the Line does not exist in the Board</response>
        [HttpDelete("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(long id, long boardId) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.WriteBoadPolicy)) {
                _logger.LogWarning(LoggingEvents.DeleteLineNotAuthorized, "Delete({id}, {boardId}) NOT AUTHORIZED {user_id}", id, boardId, User.GetNameIdentifier());
                return Challenge();
            }

            if(!await _lineService.ExistsAsync(id, boardId)) {
                //$"The line with id {id}, belonging to board with id {boardId}, does not exist"
                _logger.LogWarning(LoggingEvents.DeleteLineNotFound, "Delete({id}, {boarId}) NOT FOUND", id, boardId);
                return NotFound();
            }

            await _lineService.DeleteAsync(id, boardId);
            _logger.LogInformation(LoggingEvents.DeleteLine, "Line {id} of Board {boardId} Deleted", id, boardId);


            return NoContent();
        }
    }
}
