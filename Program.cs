using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using MorbositesBotApi.Services;

namespace MorbositesBotApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();


            var botService = host.Services.GetService(typeof(IBotService)) as IBotService;

            botService.StartBotAsync();

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
