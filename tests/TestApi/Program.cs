using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Byndyusoft.TestApi
{
    public class Program
    {
        public static void Main()
        {
            Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
                .Build()
                .Run();
        }
    }
}
