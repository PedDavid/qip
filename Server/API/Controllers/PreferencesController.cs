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
using System.Threading.Tasks;

namespace API.Controllers {
    [ValidateModel]
    [ServicesExceptionFilter]
    [Route("api/Users/{userId}/[controller]")]
    public class PreferencesController : Controller {
        private readonly IPreferencesService _preferencesService;
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger<PreferencesController> _logger;

        public PreferencesController(IPreferencesService preferencesService, IAuthorizationService authorizationService, ILogger<PreferencesController> logger) {
            _preferencesService = preferencesService;
            _authorizationService = authorizationService;
            _logger = logger;
        }

        [HttpGet(Name = "GetPreferences")]
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
                //Ideia fornecer default preferences
            }

            return Ok(preferences?.Out());//O método OK ao receber null transforma o OK(200) num NoContent(204)
        }

        [HttpPut]
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

        [HttpDelete]
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
