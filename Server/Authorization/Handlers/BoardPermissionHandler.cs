﻿using QIP.Domain;
using QIP.Public.IServices;
using QIP.Authorization.Extensions;
using QIP.Authorization.Requirements;
using QIP.Authorization.Resources;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace QIP.Authorization.Handlers {
    public class BoardPermissionHandler : AuthorizationHandler<BoardPermissionRequirement, BoardRequest> {
        private readonly IUsersBoardsService _usersBoardsService;

        public BoardPermissionHandler(IUsersBoardsService usersBoardsService) {
            _usersBoardsService = usersBoardsService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, BoardPermissionRequirement requirement, BoardRequest resource) {
            if(context.User == null || resource == null) {
                return;
            }

            string userId = context.User.GetNameIdentifier();

            BoardPermission permission = await _usersBoardsService.GetPermissionAsync(userId, resource.BoardId);

            if(permission >= requirement.Permission) {
                context.Succeed(requirement);
            } 
        }
    }
}
