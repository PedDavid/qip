using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using API.Repositories;
using WebSockets.StringWebSockets;
using API.Repositories.Extensions.DependencyInjection;
using API.Services;

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
        public void ConfigureServices(IServiceCollection services) {//TODO Ler ao fazer os Tests: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection
            // Add the CORS services
            services.AddCors(options => {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder//TODO config
                                .WithOrigins("http://localhost:51018")
                                .AllowAnyMethod()
                                .AllowAnyHeader()
                );
            });

            // Adds services required for using options.
            services.AddOptions();

            // Configure using a sub-section of the appsettings.json file.
            services.Configure<RepositoriesOptions>(Configuration.GetSection("ConnectionStrings"));

            // Add framework services.
            services.AddMvc();

            //Add Db Repositories
            services.AddRepositories();

            //Add StringWebSockets Session Manager
            services.AddSingleton<StringWebSocketsSessionManager>();

            //Add Generator of Figure Ids
            services.AddSingleton(provider => FigureIdGenerator.Create(provider.GetService<FigureIdRepository>()));
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
