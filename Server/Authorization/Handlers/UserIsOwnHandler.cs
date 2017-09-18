using QIP.Authorization.Extensions;
using QIP.Authorization.Requirements;
using QIP.Authorization.Resources;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QIP.Authorization.Handlers {
    public class UserIsOwnHandler : AuthorizationHandler<UserIsOwnRequirement, UserRequest> {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserIsOwnRequirement requirement, UserRequest resource) {
            if(context.User == null || resource == null) {
                return Task.CompletedTask;
            }

            string userId = context.User.GetNameIdentifier();

            if(resource.UserId == userId) {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
