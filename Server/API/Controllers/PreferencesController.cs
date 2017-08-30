using API.Filters;
using API.Interfaces.IServices;
using Authorization;
using Authorization.Resources;
using IODomain.Input;
using IODomain.Output;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers {
    [ServicesExceptionFilter]
    [Route("api/Users/{userId}/[controller]")]
    public class PreferencesController : Controller {
        private readonly IPreferencesService _preferencesService;
        private readonly IUserService _userService;
        private readonly IUsersBoardsService _usersBoardsService;
        private readonly IAuthorizationService _authorizationService;

        public PreferencesController(IPreferencesService preferencesService, IUserService userService, IUsersBoardsService usersBoardsService, IAuthorizationService authorizationService) {
            _preferencesService = preferencesService;
            _userService = userService;
            _usersBoardsService = usersBoardsService;
            _authorizationService = authorizationService;
        }

        [HttpGet(Name = "GetPreferences")]
        public async Task<IActionResult> GetById(string userId) {
            if(!await _authorizationService.AuthorizeAsync(User, new UserRequest(userId), Policies.UserIsOwnPolicy))
                return new ChallengeResult();

            OutPreferences outPreferences = await _preferencesService.GetAsync(userId);

            return new OkObjectResult(outPreferences);
        }

        [HttpPut]
        public async Task<IActionResult> CreateOrUpdate(string userId, [FromBody] InPreferences inPreferences) {
            if(!await _authorizationService.AuthorizeAsync(User, new UserRequest(userId), Policies.UserIsOwnPolicy))
                return new ChallengeResult();

            OutPreferences resourceCreated = await _preferencesService.CreateOrUpdateAsync(userId, inPreferences);

            if(resourceCreated != null)
                return CreatedAtRoute("GetPreferences", new { userId = userId }, resourceCreated);
            
            return new NoContentResult();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string userId) {
            if(!await _authorizationService.AuthorizeAsync(User, new UserRequest(userId), Policies.UserIsOwnPolicy))
                return new ChallengeResult();

            await _preferencesService.DeleteAsync(userId);
            return new NoContentResult();
        }
    }
}
