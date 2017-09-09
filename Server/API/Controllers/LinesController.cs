using API.Domain;
using API.Filters;
using API.Interfaces;
using API.Interfaces.IServices;
using Authorization;
using Authorization.Extensions;
using Authorization.Resources;
using IODomain.Extensions;
using IODomain.Input;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers {
    [ValidateModel]
    [ServicesExceptionFilter]
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

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(long boardId) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.ReadBoardPolicy)) {
                _logger.LogWarning(LoggingEvents.ListLinesNotAuthorized, "GetAll({boardId}) NOT AUTHORIZED {user_id}", boardId, User.GetNameIdentifier());
                return Challenge();
            }

            _logger.LogInformation(LoggingEvents.ListLines, "Listing all Lines");
            IEnumerable<Line> lines = await _lineService.GetAllAsync(boardId);

            return Ok(lines.Select(LineExtensions.Out));
        }

        [HttpGet("{id}", Name = "GetLine")]
        [AllowAnonymous]
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

        [HttpPost]
        [AllowAnonymous]
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


        [HttpPut("{id}")]
        [AllowAnonymous]
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

        [HttpDelete("{id}")]
        [AllowAnonymous]
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
