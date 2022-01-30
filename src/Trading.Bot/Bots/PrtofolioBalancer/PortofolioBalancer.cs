using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trading.Bot.Repositories;
using Trading.Bot.ServerClients;

namespace Trading.Bot.Bots.PrtofolioBalancer
{
    public class PortofolioBalancer : TradingBotBase<PortofolioBalancer, PortofolioBalancerOptions>
    {

        public PortofolioBalancer(ILogger<PortofolioBalancer> logger, TradeHistoryRepository tradeHistoryRepository)
            : base(logger, tradeHistoryRepository, Enums.BotName.PORTOFOLIO_BALANCER)
        { }

        protected override void SetBotIdentifier()
        {
            var botId = $"{Botname}.{BotOptions.Exchange}";
            foreach (var asset in BotOptions.Allocation)
            {
                botId += $".{asset.Name}.{asset.Percantage}";
            }
            BotIdentifier = botId;
        }

        protected override Task ExecuteBotStepsAsync(CancellationToken canceltToken)
        {
            throw new NotImplementedException();
        }

        protected override Task ExitingBotAsync()
        {
            throw new NotImplementedException();
        }
    }
}
