using QIP.Domain;
using Microsoft.AspNetCore.Authorization;

namespace QIP.Authorization.Requirements {
    public class BoardPermissionRequirement : IAuthorizationRequirement {
        public BoardPermission Permission { get; }

        public BoardPermissionRequirement(BoardPermission permission) {
            Permission = permission;
        }
    }
}
