using QIP.Authorization.Extensions;
using QIP.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QIP.Authorization.Handlers {
    public class AdminHandler : AuthorizationHandler<IAuthorizationRequirement> {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IAuthorizationRequirement requirement) {
            if(context.User.HasScope("Admin"))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
