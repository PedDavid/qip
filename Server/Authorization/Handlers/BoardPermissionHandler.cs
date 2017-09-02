﻿using Authorization.Requirements;
using Authorization.Resources;
using API.Interfaces.IServices;
using IODomain.Output;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.Interfaces.ServicesExceptions;

namespace Authorization.Handlers {
    public class BoardPermissionHandler : AuthorizationHandler<BoardPermissionRequirement, BoardRequest> {
        private readonly IUsersBoardsService _usersBoardsService;

        public BoardPermissionHandler(IUsersBoardsService usersBoardsService) {
            _usersBoardsService = usersBoardsService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, BoardPermissionRequirement requirement, BoardRequest resource) {
            if(context.User == null || resource == null) {
                return;
            }

            Claim nameIdentifierClaim = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            string userId = nameIdentifierClaim?.Value;

            OutBoardPermission permission = await _usersBoardsService.GetPermissionAsync(userId, resource.BoardId);

            if(permission != 0 && permission >= requirement.Permission) {
                context.Succeed(requirement);
            } 
        }
    }
}
