using QIP.API.Filters;
using QIP.Public;
using QIP.Public.IServices;
using Authorization;
using Authorization.Extensions;
using Authorization.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QIP.API.Controllers {
    [Route("api/boards/{boardId}/[controller]")]
    [Produces("application/json")]
    [ServicesExceptionFilter]
    public class FiguresController : Controller {
        private readonly IFiguresService _figuresService;
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger<FiguresController> _logger;

        public FiguresController(IFiguresService figuresService, IAuthorizationService authorizationService, ILogger<FiguresController> logger) {
            _figuresService = figuresService;
            _authorizationService = authorizationService;
            _logger = logger;
        }

        /// <summary>
        /// Deletes a range of Figures
        /// </summary>
        /// <param name="boardId">Id of the Board to delete Figures</param>
        /// <param name="lastFigureToDelete">Id of the Last Figure to delete</param>
        /// <response code="204">Board's Figures deletion succeeds</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have authorization</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> Delete(long boardId, long? lastFigureToDelete) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.BoardIsOwnPolicy)) {
                _logger.LogWarning(LoggingEvents.DeleteFiguresNotAuthorized, "Delete({boardId}) NOT AUTHORIZED {user_id}", boardId, User.GetNameIdentifier());
                return Challenge();
            }

            if(lastFigureToDelete.HasValue) {
                long lastFigToDelete = lastFigureToDelete.Value;
                await _figuresService.DeleteAsync(boardId, lastFigureToDelete.Value);
                _logger.LogInformation(LoggingEvents.DeleteFigures, "Figures up to {lastFigureToDelete} of Board {boardId} Deleted", boardId, lastFigToDelete);
            }
            else {
                await _figuresService.DeleteAsync(boardId);
                _logger.LogInformation(LoggingEvents.DeleteFigures, "Figures of Board {boardId} Deleted", boardId);
            }

            return NoContent();
        }
    }
}
