using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using API.Repositories;
using WebSockets.StringWebSockets;
using API.Interfaces.IRepositories;

namespace ApiServer {
    public class Startup {
        public Startup(IHostingEnvironment env) {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            // Add the CORS services
            services.AddCors(options => {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder//TODO config
                                .WithOrigins("http://localhost:8080")
                                .AllowAnyMethod()
                                .AllowAnyHeader()
                                .WithOrigins("http://localhost:51018")
                                .AllowAnyMethod()
                                .AllowAnyHeader()
                );
            });

            // Add framework services.
            services.AddMvc();

            //TODO Rever tempos de vida: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection
            services.AddSingleton(provider => new SqlServerTemplate(provider.GetService<IConfiguration>(), "QIPContext"));

            services.AddSingleton<IConfiguration>(Configuration);

            services.AddSingleton(typeof(StringWebSocketsSessionManager));

            services.AddScoped<IFigureIdRepository, FigureIdRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IBoardRepository, BoardRepository>();
            services.AddScoped<IImageRepository, ImageRepository>();
            services.AddScoped<ILineRepository, LineRepository>(
                provider => new LineRepository(
                    provider.GetService<SqlServerTemplate>(),
                    provider.GetService<IConfiguration>(), 
                    "QIPContext"
                    )
            );
            services.AddScoped<ILineStyleRepository, LineStyleRepository>();
            services.AddScoped<IPointStyleRepository, PointStyleRepository>();
            services.AddScoped<IUsersBoardsRepository, UsersBoardsRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory) {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // Shows UseCors with named policy.
            app.UseCors("AllowSpecificOrigin");

            var webSocketOptions = new WebSocketOptions() {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            };
            app.UseWebSockets(webSocketOptions);

            app.UseMvc();
        }
    }
}
