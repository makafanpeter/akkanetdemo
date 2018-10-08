using AkkaNetDemo.Persistance;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace AkkaNetDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args)
                .Build().MigrateDbContext<ProductContext>((context, services) =>
                {
                    var env = services.GetService<IHostingEnvironment>();

                    var logger = services.GetService<ILogger<ProductContext>>();

                    new ProductContextSeed()
                        .SeedAsync(context, env, logger)
                        .Wait();

                })
                .Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
