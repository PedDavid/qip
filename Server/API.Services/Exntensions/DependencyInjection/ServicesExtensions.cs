using API.Interfaces.IServices;
using Microsoft.Extensions.DependencyInjection;

namespace API.Services.Extensions.DependencyInjection {
    public static class ServicesExtensions {
        public static IServiceCollection AddServices(this IServiceCollection services) {
            services.AddScoped<IFigureIdService, FigureIdService>();
            services.AddScoped<IBoardService, BoardService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<ILineService, LineService>();
            services.AddScoped<ILineStyleService, LineStyleService>();
            services.AddScoped<IUsersBoardsService, UsersBoardsService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPreferencesService, PreferencesService>();

            return services;
        }
    }
}
