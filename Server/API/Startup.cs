using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using API.Models.IRepositories;
using API.Models.Repositories;

namespace API {
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
                                .WithOrigins("http://example.com")
                                .AllowAnyMethod()
                                .AllowAnyHeader()
                );
            });

            // Add framework services.
            services.AddMvc();

            //TODO Rever tempos de vida
            services.AddSingleton(new SqlServerTemplate(Configuration));

            services.AddSingleton<IConfiguration>(Configuration);

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IBoardRepository, BoardRepository>();
            services.AddScoped<IImageRepository, ImageRepository>();
            services.AddScoped<ILineRepository, LineRepository>();
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

            app.UseMvc();
        }
    }
}
