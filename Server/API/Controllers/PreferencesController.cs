using API.Domain;
using API.Filters;
using API.Interfaces.IServices;
using Authorization;
using Authorization.Resources;
using IODomain.Extensions;
using IODomain.Input;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers {
    [ValidateModel]
    [ServicesExceptionFilter]
    [Route("api/Users/{userId}/[controller]")]
    public class PreferencesController : Controller {
        private readonly IPreferencesService _preferencesService;
        private readonly IAuthorizationService _authorizationService;

        public PreferencesController(IPreferencesService preferencesService, IAuthorizationService authorizationService) {
            _preferencesService = preferencesService;
            _authorizationService = authorizationService;
        }

        [HttpGet(Name = "GetPreferences")]
        public async Task<IActionResult> GetById(string userId) {
            if(!await _authorizationService.AuthorizeAsync(User, new UserRequest(userId), Policies.UserIsOwnPolicy))
                return Challenge();

            Preferences preferences = await _preferencesService.GetAsync(userId);

            if(preferences == null) {
                //TODO Ver o que fazer
                //Ideia fornecer default preferences
            }

            return Ok(preferences.Out());
        }

        [HttpPut]
        public async Task<IActionResult> CreateOrUpdate(string userId, [FromBody] InPreferences inPreferences) {
            if(!await _authorizationService.AuthorizeAsync(User, new UserRequest(userId), Policies.UserIsOwnPolicy))
                return Challenge();

            if(inPreferences == null) {
                return BadRequest();
            }

            if(inPreferences.UserId != userId) {
                return BadRequest();
            }

            Preferences preferences = new Preferences { UserId = userId }.In(inPreferences);

            bool created = await _preferencesService.CreateOrUpdateAsync(preferences);

            if(created)
                return CreatedAtRoute("GetPreferences", new { userId = userId }, preferences.Out());

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string userId) {
            if(!await _authorizationService.AuthorizeAsync(User, new UserRequest(userId), Policies.UserIsOwnPolicy))
                return Challenge();

            if(!await _preferencesService.ExistsAsync(userId)) {
                return NotFound();
            }

            await _preferencesService.DeleteAsync(userId);

            return NoContent();
        }
    }
}
