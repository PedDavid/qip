using API.Interfaces.IServices;
using Microsoft.Extensions.DependencyInjection;

namespace API.Services.Extensions.DependencyInjection {
    public static class ServicesExtensions {
        public static IServiceCollection AddServices(this IServiceCollection services) {
            services.AddScoped<IFigureIdService, FigureIdService>();

            return services;
        }
    }
}
