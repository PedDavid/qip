using IODomain.Output;
using Microsoft.AspNetCore.Authorization;

namespace Authorization.Requirements {
    public class BoardPermissionRequirement : IAuthorizationRequirement {
        public OutBoardPermission Permission { get; }

        public BoardPermissionRequirement(OutBoardPermission permission) {
            Permission = permission;
        }
    }
}
