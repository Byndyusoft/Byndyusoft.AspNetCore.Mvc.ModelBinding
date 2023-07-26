using System.IO;
using Byndyusoft.Example;
using Byndyusoft.Example.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Byndyusoft.IntegrationTests
{
    public class ApiFixture : WebApplicationFactory<Program>
    {
        public SaveFileSettings SaveFileSettings { get; } = new() { FolderName = Path.GetFullPath("Temp") };

        public T GetService<T>() where T : notnull => Services.GetRequiredService<T>();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.ConfigureTestServices(ConfigureTestServices);
            builder.UseEnvironment(Environments.Development);
        }

        private void ConfigureTestServices(IServiceCollection service)
        {
            service.AddSingleton(Options.Create(SaveFileSettings));
        }
    }
}