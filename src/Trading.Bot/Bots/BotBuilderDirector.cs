using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Bot.Bots.MarketMaker;
using Trading.Bot.Bots.PrtofolioBalancer;
using Trading.Bot.Enums;

namespace Trading.Bot.Bots
{
    public class BotBuilderDirector
    {
        private ITradingBotBuilder _builder;
        private IServiceProvider _serviceProvider;
        public BotBuilderDirector(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void SetBuilder(string bot)
        {
            switch ((BotName)Enum.Parse(typeof(BotName), bot, true))
            {
                case BotName.MARKET_MAKER:
                    _builder = new MarketMakerBuilder(_serviceProvider);
                    break;
                case BotName.PORTOFOLIO_BALANCER:
                    _builder = new PortofolioBalancerBuilder(_serviceProvider);
                    break;
                default:
                    throw new Exception($"{bot} is not implemented");
            }
        }

        public ITradingBot Build(Dictionary<string, string> options)
        {
            _builder.SetServerClient();
            _builder.SetBotOptions(options);
            return _builder.GetBot();
        }
    }
}
