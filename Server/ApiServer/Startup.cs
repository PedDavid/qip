using API.Repositories;
using API.Repositories.Extensions.DependencyInjection;
using API.Services.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using WebSockets.StringWebSockets;

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
                                .WithOrigins("http://localhost:8080")
                                .AllowAnyMethod()
                                .AllowAnyHeader()
                                .WithOrigins("http://localhost:51018")
                                .AllowAnyMethod()
                                .AllowAnyHeader()
                );
            });



            // Adds services required for using options.
            services.AddOptions();

            // Configure using a sub-section of the appsettings.json file.
            services.Configure<RepositoriesOptions>(Configuration.GetSection("ConnectionStrings"));

            //Add Memory Cache
            services.AddMemoryCache();

            // Add framework services.
            services.AddMvc(config => {
                // Define that by default authentication is required
                var policy = new AuthorizationPolicyBuilder() 
                                 .RequireAuthenticatedUser()
                                 .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });

            //Add Db Repositories
            services.AddRepositories();

            //Add Services
            services.AddServices();

            //Add StringWebSockets Session Manager
            services.AddSingleton<StringWebSocketsSessionManager>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory) {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // Shows UseCors with named policy.
            app.UseCors("AllowSpecificOrigin");

            var options = new JwtBearerOptions {
                Audience = Configuration["Auth0:ApiIdentifier"],
                Authority = $"https://{Configuration["Auth0:Domain"]}/"
            };
            app.UseJwtBearerAuthentication(options);

            var webSocketOptions = new WebSocketOptions() {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            };
            app.UseWebSockets(webSocketOptions);

            app.UseMvc();
        }
    }
}
