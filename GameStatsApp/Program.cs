using Lamar.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace GameStatsApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateWebHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                    .UseLamar()
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseStartup<Startup>();
                        webBuilder.ConfigureServices((ctx, services) =>
                        {
                            services.AddControllers();
                        });
                        // webBuilder.UseUrls("http://*:5001");
                    })
                    .UseSerilog((hostingContext, loggerConfiguration) =>
                    {
                        loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
                    });
    }
}


