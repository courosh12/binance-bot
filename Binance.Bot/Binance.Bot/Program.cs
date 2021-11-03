using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Binance.Bot.Data;
using Binance.Net;
using Binance.Net.Enums;
using Binance.Net.Objects;
using CryptoExchange.Net.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.AspNetCore;
using Serilog.Events;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Binance.Bot
{
    
    class Program
    {
        private static IConfiguration _configuration;
        static async Task Main(string[] args)
        {
            _configuration =  new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(_configuration)
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
                    configHost.AddEnvironmentVariables();
                });

        public static void ConfigureService(IServiceCollection serviceCollection)
        {
            var socketClient = new BinanceSocketClient(new BinanceSocketClientOptions()
            {
                
                LogLevel = LogLevel.Information,
                LogWriters = new List<ILogger> { new SerilogLoggerFactory(Log.Logger).CreateLogger<Program>() },
                // Specify options for the client
            });
                
            var settings = _configuration.GetSection("ApiKeys");

            var client = new BinanceClient(new BinanceClientOptions(){
                // Specify options for the client
                AutoTimestamp = true,
                TradeRulesBehaviour = TradeRulesBehaviour.AutoComply,
                LogWriters = new List<ILogger> { new SerilogLoggerFactory(Log.Logger).CreateLogger<Program>() },
                ApiCredentials = new ApiCredentials(settings.GetSection("Key").Value, 
                    settings.GetSection("Secret").Value)
            });
            
            serviceCollection.AddSingleton(socketClient);
            serviceCollection.AddSingleton(client);
            serviceCollection.AddDbContextFactory<TradeContext>(options =>
            {
                options.UseSqlite(_configuration.GetConnectionString("DefaultConnection"));
            });
            
            serviceCollection.AddTransient<TradesService>();
            serviceCollection.AddTransient<VolatilityBot,VolatilityBot>();
            
        }
    }
}
