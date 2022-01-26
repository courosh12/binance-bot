using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Trading.Bot.Data;
using Binance.Net;
using Binance.Net.Enums;
using Binance.Net.Objects;
using CryptoExchange.Net.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using Trading.Bot.Services;
using Serilog.Extensions.Logging;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Trading.Bot.Bots;

namespace Trading.Bot
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
                })
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddJsonFile("appsettings.json", optional: false);
                    configHost.AddEnvironmentVariables();
                });

        public static void ConfigureService(IServiceCollection serviceCollection)
        {
            AddBinance(serviceCollection);

            serviceCollection.AddDbContextFactory<TradeContext>(options =>
            {
                options.UseSqlite(_configuration.GetConnectionString("DefaultConnection"));
            });

            AddBotConfiguration(serviceCollection);
            serviceCollection.AddTransient<BotBuilderDirector>();
            serviceCollection.AddHostedService<BotHostedService>();
        }

        private static void AddBinance(IServiceCollection serviceCollection)
        {
            var settings = _configuration.GetSection("ApiKeys");
            var client = new BinanceClient(new BinanceClientOptions()
            {
                AutoTimestamp = true,
                TradeRulesBehaviour = TradeRulesBehaviour.AutoComply,
                LogWriters = new List<ILogger> { new SerilogLoggerFactory(Log.Logger).CreateLogger<Program>() },
                ApiCredentials = new ApiCredentials(settings.GetSection("Key").Value, settings.GetSection("Secret").Value)
            });
            serviceCollection.AddSingleton(client);
        }

        private static void AddBotConfiguration(IServiceCollection serviceCollection)
        {
            var botsSettings = _configuration.GetSection("BotOptions").GetChildren().ToList()
               .Select(p => new BotOptions()
               {
                   Name = p.GetSection("Name").Value,
                   Setting = p.GetSection("Setting").GetChildren().Select(p => new KeyValuePair<string, string>(p.Key, p.Value)).ToDictionary(p => p.Key, p => p.Value)
               }).ToList();

            serviceCollection.AddSingleton<List<BotOptions>>(botsSettings);
        }


    }
}
