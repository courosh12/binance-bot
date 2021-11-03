using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance.Bot.Data;
using Binance.Net;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SQLitePCL;
using ILogger = Serilog.ILogger;

namespace Binance.Bot
{
    public class BotHostedService:IHostedService
    {
        private ILogger<BotHostedService> _logger;
        private IConfiguration _conf;
        private IServiceProvider _service;
        private List<IBot> _bots;
        private IDbContextFactory<TradeContext> _trFact;

        public BotHostedService(ILogger<BotHostedService> logger,IConfiguration conf,IServiceProvider services,IDbContextFactory<TradeContext> trFact)
        {
            _logger = logger;
            _conf = conf;
            _service = services;
            _bots = new List<IBot>();
            _trFact =trFact;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            using (var context =_trFact.CreateDbContext() )
            {
                context.Database.Migrate();
                _logger.LogInformation("Staring bots");
            
                var settings = _conf.GetSection("BotSettings").Get<BotSetting[]>();
            
                foreach (var setting in settings)
                {
                    if (!context.Bots
                        .Any(p => p.Symbol == setting.Symbol && p.TimeSpan == setting.TimeSpan && p.ChangeInPrice == setting.ChangeInPrice))
                    {
                        context.Bots.Add(new Data.Bot()
                        {
                            Symbol = setting.Symbol,
                            TimeSpan = setting.TimeSpan,
                            ChangeInPrice = setting.ChangeInPrice
                        });
                        context.SaveChanges();
                    }
                    _bots.Add(
                        new VolatilityBot(_service.GetService<BinanceSocketClient>(),
                            _service.GetService<BinanceClient>(),
                            _service.GetService<ILogger<VolatilityBot>>(),
                            setting,PrintAllTraddeInfo,
                            _service.GetService<TradesService>())
                    );
                }

                foreach (var bot in _bots)
                {
                    bot.SubscribeToData();
                }
            }
            

            
            return Task.CompletedTask;
        }

        private void PrintAllTraddeInfo()
        {
            foreach (var bot in _bots)
            {
                bot.ShowAverageBuySell();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Shutting down");
            return Task.CompletedTask;
        }
    }
}