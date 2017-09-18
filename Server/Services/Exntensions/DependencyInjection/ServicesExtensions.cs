using QIP.Public.IServices;
using Microsoft.Extensions.DependencyInjection;

namespace QIP.Services.Extensions.DependencyInjection {
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
            services.AddScoped<IFiguresService, FiguresService>();

            return services;
        }
    }
}
