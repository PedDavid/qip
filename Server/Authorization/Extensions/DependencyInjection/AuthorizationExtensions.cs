using Authorization.Handlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Authorization.Extensions.DependencyInjection {
    public static class AuthorizationExtensions {
        public static IServiceCollection AddApiAuthorization(this IServiceCollection services) {
            services.AddSingleton<IAuthorizationHandler, AdminHandler>();
            services.AddSingleton<IAuthorizationHandler, UserIsOwnHandler>();
            services.AddSingleton<IAuthorizationHandler, BoardPermissionHandler>();

            return services;
        }
    }
}
