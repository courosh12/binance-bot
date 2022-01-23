using Binance.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trading.Bot.ServerClients
{
    internal class BinanceRestClient : ITradingClient
    {

        private readonly BinanceClient _client;

        public BinanceRestClient(BinanceClient binanceClient)
        {
            _client = binanceClient;
        }

        public async Task<decimal> GetHighestBidOrderAsync(string symbol)
        {
            var orderBookCall = await _client.Spot.Market.GetOrderBookAsync(symbol, 1);
            if(orderBookCall.Success)
            {
                return orderBookCall.Data.Bids.OrderBy(p => p.Price).First().Price;
            }
            else
            {
                throw new NotImplementedException("you forgot to error handle :(");
            }
        }

        public async Task<decimal> GetHighestAskOrderAsync(string symbol)
        {
            var orderBookCall = await _client.Spot.Market.GetOrderBookAsync(symbol, 1);
            if (orderBookCall.Success)
            {
                return orderBookCall.Data.Asks.OrderBy(p => p.Price).First().Price;
            }
            else
            {
                throw new NotImplementedException("you forgot to error handle :(");
            }
        }
    }
}
