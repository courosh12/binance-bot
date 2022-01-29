using Binance.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Bot.Bots;
using Trading.Bot.Data;
using Trading.Bot.Enums;
using Trading.Bot.Repositories;
using Trading.Bot.ServerClients;

namespace Trading.Bot.Bots.MarketMaker
{
    public class MarketMakerBuilder : ITradingBotBuilder
    {
        private ITradingBot<MarketMakerOptions> _bot;
        private IServiceProvider _serviceProvider;
        public MarketMakerBuilder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            Reset();
        }

        public void Reset()
        {
            _bot = new MarketMaker(_serviceProvider.GetService<ILogger<MarketMaker>>(),
                _serviceProvider.GetService<TradeHistoryRepository>());
        }

        public void SetServerClient()
        {
            _bot.TradingClient = new BinanceRestClient(_serviceProvider.GetService<BinanceClient>(),
                _serviceProvider.GetService<ILogger<BinanceRestClient>>());
        }

        public void SetBotOptions(Dictionary<string, string> settings)
        {
            var mmOptions = new MarketMakerOptions();
            mmOptions.Symbol = settings["Symbol"];
            mmOptions.Spread = decimal.Parse(settings["Spread"]);
            mmOptions.Ordervalue = decimal.Parse(settings["OrderValue"]);
            mmOptions.OrderQuantity = int.Parse(settings["OrderQuantity"]);
            mmOptions.Interval = int.Parse(settings["Interval"]);

            _bot.BotOptions = mmOptions;
        }

        public ITradingBot GetBot()
        {
            return _bot;
        }
    }
}
