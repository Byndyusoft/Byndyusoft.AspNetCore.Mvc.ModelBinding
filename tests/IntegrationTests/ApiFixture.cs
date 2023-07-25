using Byndyusoft.Example;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Byndyusoft.IntegrationTests
{
    public class ApiFixture : WebApplicationFactory<Program>
    {
        public T GetService<T>() where T : notnull => Services.GetRequiredService<T>();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.UseEnvironment(Environments.Development);
        }
    }
}