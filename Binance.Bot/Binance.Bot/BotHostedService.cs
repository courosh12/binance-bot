using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Binance.Net;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ILogger = Serilog.ILogger;

namespace Binance.Bot
{
    public class BotHostedService:IHostedService
    {
        private ILogger<BotHostedService> _logger;
        private IConfiguration _conf;
        private IServiceProvider _service;
        private List<IBot> _bots;

        public BotHostedService(ILogger<BotHostedService> logger,IConfiguration conf,IServiceProvider services)
        {
            _logger = logger;
            _conf = conf;
            _service = services;
            _bots = new List<IBot>();
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Staring bots");
            
            var settings = _conf.GetSection("BotSettings").Get<BotSetting[]>();
            
            foreach (var setting in settings)
            {
                _bots.Add(
                    new VolatilityBot(_service.GetService<BinanceSocketClient>(),
                    _service.GetService<BinanceClient>(),
                    _service.GetService<ILogger<VolatilityBot>>(),
                    setting)
                );
            }

            foreach (var bot in _bots)
            {
                bot.SubscribeToData();
            }
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}