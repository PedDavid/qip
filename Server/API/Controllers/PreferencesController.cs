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
using System.Threading.Tasks;

namespace API.Controllers {
    [ValidateModel]
    [ServicesExceptionFilter]
    [Route("api/Users/{userId}/[controller]")]
    [Produces("application/json")]
    public class PreferencesController : Controller {
        private readonly IPreferencesService _preferencesService;
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger<PreferencesController> _logger;

        public PreferencesController(IPreferencesService preferencesService, IAuthorizationService authorizationService, ILogger<PreferencesController> logger) {
            _preferencesService = preferencesService;
            _authorizationService = authorizationService;
            _logger = logger;
        }

        /// <summary>
        /// Returns a specific user's Preferences.
        /// </summary>
        /// <param name="userId">Id of the user to whom the preferences belong</param>
        /// <returns>Required Preferences</returns>
        /// <response code="200">Returns the required Preferences</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have authorization</response>
        [HttpGet(Name = "GetPreferences")]
        [ProducesResponseType(typeof(OutPreferences), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetById(string userId) {
            if(!await _authorizationService.AuthorizeAsync(User, new UserRequest(userId), Policies.UserIsOwnPolicy)) {
                _logger.LogWarning(LoggingEvents.GetPreferencesNotAuthorized, "GetById({userId}) NOT AUTHORIZED {user_id}", userId, User.GetNameIdentifier());
                return Challenge();
            }

            _logger.LogInformation(LoggingEvents.GetPreferences, "Getting Preferences {userId}", userId);
            Preferences preferences = await _preferencesService.GetAsync(userId);

            if(preferences == null) {
                _logger.LogWarning(LoggingEvents.GetPreferencesNotFound, "GetById({userId}) NOT FOUND", userId);
                //TODO Ver o que fazer
                //Ideia: fornecer default preferences
            }

            return Ok(preferences?.Out());//O método OK ao receber null transforma o OK(200) num NoContent(204)
        }

        /// <summary>
        /// Update a user's Preferences, creating them if they do not exist
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/Users/auth0%7C1234567890/Preferences
        ///     Content-Type: application/json
        ///     Authorization: Bearer {ACCESS_TOKEN}
        /// 
        ///     {
        ///         "UserId": "auth0|1234567890",
        ///         "Favorites": "favoritesExample",
        ///         "PenColors": "penColorsExample",
        ///         "DefaultPen": "defaultPenExample",
        ///         "DefaultEraser": "defaultEraserExample",
        ///         "CurrTool": "currToolExample",
        ///         "Settings": "settingsExample"
        ///     }
        ///
        /// </remarks>
        /// <param name="userId">Id of the user to whom the preferences belong</param>
        /// <param name="inPreferences">Information of the Preferences to update</param>
        /// <returns>A newly-created Board</returns>
        /// <response code="201">Returns the newly-created Board</response>
        /// <response code="204">Board update succeeds</response>
        /// <response code="400">If there is inconsistent information or the inPreferences is null</response> 
        [HttpPut]
        [ProducesResponseType(typeof(OutPreferences), 201)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateOrUpdate(string userId, [FromBody] InPreferences inPreferences) {
            if(!await _authorizationService.AuthorizeAsync(User, new UserRequest(userId), Policies.UserIsOwnPolicy)) {
                _logger.LogWarning(LoggingEvents.InsertUpdatePreferencesNotAuthorized, "CreateOrUpdate({userId}) NOT AUTHORIZED {user_id}", userId, User.GetNameIdentifier());
                return Challenge();
            }

            if(inPreferences == null) {
                _logger.LogDebug(LoggingEvents.InsertUpdatePreferencesWithoutBody, "CreateOrUpdate({userId}) WITHOUT BODY", userId);
                return BadRequest();
            }

            if(inPreferences.UserId != userId) {
                _logger.LogDebug(LoggingEvents.InsertUpdatePreferencesWrongId, "CreateOrUpdate({userId}) WRONG ID {wrongId}", userId, inPreferences.UserId);
                return BadRequest();
            }

            Preferences preferences = new Preferences { UserId = userId }.In(inPreferences);

            bool created = await _preferencesService.CreateOrUpdateAsync(preferences);

            if(created) {
                _logger.LogInformation(LoggingEvents.InsertPreferences, "Preferences {userId} Created", userId);
                return CreatedAtRoute("GetPreferences", new { userId = userId }, preferences.Out());
            }

            _logger.LogInformation(LoggingEvents.UpdatePreferences, "Preferences {userId} Updated", userId);
            return NoContent();
        }

        /// <summary>
        /// Deletes a specific user's Preferences.
        /// </summary>
        /// <param name="userId">Id of the user whose preferences you want to delete</param>
        /// <response code="204">User's Preferences deletion succeeds</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have authorization</response>
        /// <response code="404">If the user's Preferences not exists</response>
        [HttpDelete]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(string userId) {
            if(!await _authorizationService.AuthorizeAsync(User, new UserRequest(userId), Policies.UserIsOwnPolicy)) {
                _logger.LogWarning(LoggingEvents.DeletePreferencesNotAuthorized, "Delete({userId}) NOT AUTHORIZED {user_id}", userId, User.GetNameIdentifier());
                return Challenge();
            }

            if(!await _preferencesService.ExistsAsync(userId)) {
                _logger.LogWarning(LoggingEvents.DeletePreferencesNotFound, "Delete({userId}) NOT FOUND", userId);
                return NotFound();
            }

            await _preferencesService.DeleteAsync(userId);
            _logger.LogInformation(LoggingEvents.DeletePreferences, "Preferences {userId} Deleted", userId);

            return NoContent();
        }
    }
}
