using System;
using System.IO;
using System.Threading.Tasks;
using Binance.Net;
using Binance.Net.Objects;
using CryptoExchange.Net.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Binance.Bot
{
    
    class Program
    {
        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                Log.Information("Starting host");
                using IHost host = CreateHostBuilder(args).Build();
                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureServices((hostcontext, services) =>
                {
                    ConfigureService((services));
                    services.AddHostedService<BotHostedService>();
                })
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddJsonFile("appsettings.json", optional: false);
                });

        public static void ConfigureService(IServiceCollection serviceCollection)
        {
            var socketClient = new BinanceSocketClient(new BinanceSocketClientOptions()
            {
                // Specify options for the client
            });
            
            var client = new BinanceClient(new BinanceClientOptions(){
                // Specify options for the client
                ApiCredentials = new ApiCredentials("JUV9CtM1On6hnDBhdD5B31epV2vnYVP5oletXMGPojbzz0uTzzXGMQpj4pgmyawm", 
                    "M4wTQ1q7h021drW5oZvluX0Ci2yd5TfROeMnmwjzAupPHW3kU4tuNGOP1uW7rjE1")
            });
            
            serviceCollection.AddSingleton(socketClient);
            serviceCollection.AddSingleton(client);

            serviceCollection.AddTransient<VolatilityBot,VolatilityBot>();
            
        }



    }
}
