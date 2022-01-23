using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Bot.ServerClients;

namespace Trading.Bot.Bots.MarketMaker
{
    public class MarketMaker : ITradingBot
    {
        public ITradingClient TradingClient { get; set; }
        public MarketMakerSettings Settings { get; set; }

        public async Task StartAsync()
        {
            var highestBid = await TradingClient.GetHighestBidOrderAsync(Settings.Symbol);
            var hisghestAsk = await TradingClient.GetHighestAskOrderAsync(Settings.Symbol);
            //get data?
            //cancel open orders
            //new orders
            //profit?
        }
    }
}
