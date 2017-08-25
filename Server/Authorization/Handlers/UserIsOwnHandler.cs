using Authorization.Requirements;
using Authorization.Resources;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Authorization.Handlers {
    public class UserIsOwnHandler : AuthorizationHandler<UserIsOwnRequirement, UserRequest> {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserIsOwnRequirement requirement, UserRequest resource) {
            if(context.User == null || resource == null) {
                return Task.CompletedTask;
            }

            Claim nameIdentifierClaim = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            string userId = nameIdentifierClaim?.Value; 

            if(resource.UserId == userId) {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
