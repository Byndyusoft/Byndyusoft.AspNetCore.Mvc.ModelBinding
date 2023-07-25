using System;
using Byndyusoft.Example.Services;
using Byndyusoft.Example.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Byndyusoft.Example
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(o => o.AddFormStreamedFileCollectionBinder());
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddSingleton<FileService>();
            services.Configure<SaveFileSettings>(Configuration.GetSection(nameof(SaveFileSettings)));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var environmentVariables = Environment.GetEnvironmentVariables();

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}