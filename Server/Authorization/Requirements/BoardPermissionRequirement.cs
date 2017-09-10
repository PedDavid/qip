﻿using API.Domain;
using Microsoft.AspNetCore.Authorization;

namespace Authorization.Requirements {
    public class BoardPermissionRequirement : IAuthorizationRequirement {
        public BoardPermission Permission { get; }

        public BoardPermissionRequirement(BoardPermission permission) {
            Permission = permission;
        }
    }
}
