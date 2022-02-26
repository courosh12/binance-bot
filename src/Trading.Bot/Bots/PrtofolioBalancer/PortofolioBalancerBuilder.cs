using Binance.Net;
using Binance.Net.Clients;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Bot.Models;
using Trading.Bot.Repositories;
using Trading.Bot.ServerClients;

namespace Trading.Bot.Bots.PrtofolioBalancer
{
    public class PortofolioBalancerBuilder : ITradingBotBuilder
    {
        private IServiceProvider _serviceProvider;
        private ITradingBot<PortofolioBalancerOptions> _bot;

        public PortofolioBalancerBuilder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            Reset();
        }

        public ITradingBot GetBot()
        {
            return _bot;
        }

        public void Reset()
        {
            _bot = new PortofolioBalancer(_serviceProvider.GetService<ILogger<PortofolioBalancer>>(),
                _serviceProvider.GetService<TradeHistoryRepository>());
        }

        public void SetBotOptions(Dictionary<string, string> setting)
        {
            var allocation = new List<AssetAllocation>();
            foreach (var asset in setting["Allocation"].Split(','))
            {
                allocation.Add(new AssetAllocation()
                {
                    Name = asset.Split('.')[0],
                    Percantage = decimal.Parse(asset.Split('.')[1])
                });
            }

            var options = new PortofolioBalancerOptions()
            {
                Market = setting["Market"],
                Interval = int.Parse(setting["Interval"]),
                Trigger = decimal.Parse(setting["Trigger"]),
                Allocation = allocation
            };

            _bot.BotOptions = options;
        }

        public void SetServerClient()
        {
            _bot.TradingClient = new BinanceRestClient(_serviceProvider.GetService<BinanceClient>(),
                _serviceProvider.GetService<ILogger<BinanceRestClient>>());
        }
    }
}
