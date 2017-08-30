﻿using System.Linq;
using System.Security.Claims;

namespace Authorization.Extensions {
    public static class ClaimsPrincipalExtensions {
        public static bool HasScope(this ClaimsPrincipal principal, string scope) {
            // If user does not have the scope claim, get out of here
            if(!principal.HasClaim(c => c.Type == "scope"))
                return false;

            // Split the scopes string into an array
            var scopes = principal.FindFirst(c => c.Type == "scope").Value.Split(' ');

            // Succeed if the scope array contains the required scope
            return scopes.Any(s => s == scope);
        }
    }
}