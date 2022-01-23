using Binance.Net;
using Microsoft.Extensions.DependencyInjection;
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
            _bot = new MarketMaker();
        }

        public void SetServerClient()
        {
            _bot.TradingClient = new BinanceRestClient(_serviceProvider.GetService<BinanceClient>());
        }

        public void SetSettings(Dictionary<string,string> settings)
        {
            var mmSetting = new MarketMakerSettings();
            mmSetting.Symbol = settings["Symbol"];
            _bot.Settings = mmSetting;
        }

        public ITradingBot GetBot()
        {
            return _bot;
        }
    }
}
