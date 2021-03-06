﻿using QIP.Authorization;
using QIP.Authorization.Requirements;
using QIP.Authorization.Extensions.DependencyInjection;
using QIP.Repositories.Options;
using QIP.Repositories.Extensions.DependencyInjection;
using QIP.Services.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using QIP.WebSockets.StringWebSockets;
using Newtonsoft.Json.Converters;
using Microsoft.Extensions.Primitives;
using System.Linq;
using QIP.Domain;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;

namespace QIP.ApiServer {
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

            //Add Authorization Policies
            string domain = $"https://{Configuration["Auth0:Domain"]}/";//TODO Config on the appsettings
            services.AddAuthorization(options => {
                options.AddPolicy("Administrator", policy => policy.Requirements.Add(new AdminRequirement()));
                options.AddPolicy(Policies.UserIsOwnPolicy, policy => policy.Requirements.Add(new UserIsOwnRequirement()));
                options.AddPolicy(Policies.BoardIsOwnPolicy, policy => policy.Requirements.Add(new BoardPermissionRequirement(BoardPermission.Owner)));
                options.AddPolicy(Policies.ReadBoardPolicy, policy => policy.Requirements.Add(new BoardPermissionRequirement(BoardPermission.View)));
                options.AddPolicy(Policies.WriteBoadPolicy, policy => policy.Requirements.Add(new BoardPermissionRequirement(BoardPermission.Edit)));
            });

            //Add Api Authorization Handlers
            services.AddApiAuthorization();

            // Adds services required for using options.
            services.AddOptions();

            // Configure using a sub-section of the appsettings.json file.
            services.Configure<DatabaseOptions>(Configuration.GetSection("ConnectionStrings"));
            services.Configure<Auth0Options>(Configuration.GetSection("Auth0"));

            //Add Memory Cache
            services.AddMemoryCache();

            // Add framework services.
            services.AddMvc(config => {
                // Define that by default authentication is required
                var policy = new AuthorizationPolicyBuilder() 
                                 .RequireAuthenticatedUser()
                                 .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            })/*.AddJsonOptions(options => {
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            })*/;//TODO VER SE É PREFERIVEL

            //Add Db Repositories
            services.AddRepositories();

            //Add Services
            services.AddServices();

            //Add StringWebSockets Session Manager
            services.AddSingleton<StringWebSocketsSessionManager>();

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "QIP API", Version = "V1" });

                // Set the comments path for the Swagger JSON and UI.
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlApiPath = Path.Combine(basePath, "Api.xml");
                var xmlWebSocketsPath = Path.Combine(basePath, "WebSockets.xml");
                c.IncludeXmlComments(xmlApiPath);
                c.IncludeXmlComments(xmlWebSocketsPath);
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory) {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // Shows UseCors with named policy.
            app.UseCors("AllowSpecificOrigin");

            //Transform access_token Query Parameter in Authorization header
            app.Use(async (context, next) => {//TODO Perguntar se existe uma maneira melhor, tendo em conta que os WebSockets do JS aparentemente não suportam headers
                if(context.Request.Path.Value.StartsWith("/ws")) {
                    if(context.Request.Query.TryGetValue("access_token", out StringValues accessToken)) {
                        if(!StringValues.IsNullOrEmpty(accessToken)) {
                            var header = new StringValues(accessToken.Select(token => $"Bearer {token}").ToArray());
                            context.Request.Headers.Add("Authorization", header);
                        }
                    }
                }
                await next();
            });

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

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.ShowJsonEditor();
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "QIP API V1");
            });

            app.UseMvc();
        }
    }
}
