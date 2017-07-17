using Microsoft.Extensions.DependencyInjection;
using API.Interfaces.IRepositories;

namespace API.Repositories.Extensions.DependencyInjection {
    public static class RepositoriesExtensions {
        public static IServiceCollection AddRepositories(this IServiceCollection services) {
            //Scoped lifetime services are created once per request.
            //Sem estado mas sem razão para criar uma instancia por utilização
            services.AddScoped<SqlServerTemplate>();

            services.AddScoped<IFigureIdRepository, FigureIdRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IBoardRepository, BoardRepository>();
            services.AddScoped<IImageRepository, ImageRepository>();
            services.AddScoped<ILineRepository, LineRepository>();
            services.AddScoped<ILineStyleRepository, LineStyleRepository>();
            services.AddScoped<IUsersBoardsRepository, UsersBoardsRepository>();

            return services;
        }
    }
}
