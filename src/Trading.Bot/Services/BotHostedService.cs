using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Trading.Bot.Data;
using Binance.Net;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SQLitePCL;
using ILogger = Serilog.ILogger;
using Microsoft.Extensions.Options;
using Trading.Bot.Bots;

namespace Trading.Bot.Services
{
    public class BotHostedService : IHostedService
    {
        private ILogger<BotHostedService> _logger;
        private List<BotOptions> _botsSettings;
        private BotBuilderDirector _director;
        private List<ITradingBot> _bots = new List<ITradingBot>();
        private List<Task> _runningBots = new List<Task>();
        private IDbContextFactory<TradeContext> _tradeDbContextFacotry;
        public BotHostedService(
            ILogger<BotHostedService> logger,
            List<BotOptions> botsSettings,
            BotBuilderDirector director,
            IDbContextFactory<TradeContext> factory
            )
        {
            _director = director;
            _botsSettings = botsSettings;
            _logger = logger;
            _tradeDbContextFacotry = factory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            MigrateDataBase();
            BuildBots();
            StartBots(cancellationToken);
            return Task.WhenAll(_runningBots.ToArray());
        }

        private void MigrateDataBase()
        {
            using var context = _tradeDbContextFacotry.CreateDbContext();
            context.Database.Migrate();
        }

        private void StartBots(CancellationToken token)
        {
            foreach (var bot in _bots)
            {
                _runningBots.Add(bot.StartAsync(token));
            }
        }

        private void BuildBots()
        {
            foreach (var botSetting in _botsSettings)
            {
                _director.SetBuilder(botSetting.Name);
                _bots.Add(_director.Build(botSetting.Setting));
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Shutting down");
            return Task.CompletedTask;
        }
    }
}