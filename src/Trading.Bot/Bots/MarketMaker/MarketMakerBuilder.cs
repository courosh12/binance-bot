using Binance.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Bot.Bots;
using Trading.Bot.ServerClients;

namespace Trading.Bot.Bots.MarketMaker
{
    public class MarketMakerBuilder : ITradingBotBuilder
    {
        private MarketMaker _bot;
        private IServiceProvider _serviceProvider;

        public MarketMakerBuilder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            Reset();
        }

        public void Reset()
        {
            _bot = new MarketMaker(_serviceProvider.GetService<ILogger<MarketMaker>>());
        }

        public void SetServerClient()
        {
            _bot.TradingClient = new BinanceRestClient(_serviceProvider.GetService<BinanceClient>(),
                _serviceProvider.GetService<ILogger<BinanceRestClient>>());
        }

        public void SetSettings(Dictionary<string, string> settings)
        {
            var mmSetting = new MarketMakerOptions();
            mmSetting.Symbol = settings["Symbol"];
            mmSetting.Difference = decimal.Parse(settings["Difference"]);
            mmSetting.Ordervalue = decimal.Parse(settings["OrderValue"]);
            mmSetting.OrderAmount = int.Parse(settings["OrderAmount"]);

            _bot.Settings = mmSetting;
        }

        public ITradingBot GetBot()
        {
            return _bot;
        }
    }
}
