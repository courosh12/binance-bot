using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Bot.Models;
using Trading.Bot.ServerClients;

namespace Trading.Bot.Bots.MarketMaker
{
    public class MarketMaker : ITradingBot
    {
        public ITradingClient TradingClient { get; set; }
        public MarketMakerOptions Settings { get; set; }

        private readonly ILogger _logger;
        private decimal _lowestAsk;
        private decimal _highestBid;
        private const int DECIMALS = 8;

        public MarketMaker(ILogger<MarketMaker> logger)
        {
            _logger = logger;
        }

        public async Task StartAsync()
        {
            await GetPrices();
            await CancelOpenOrders();
            var orders = CreateNewOrders();
            await ExecuteOrders(orders);
        }

        private async Task GetPrices()
        {
            _highestBid = await TradingClient.GetHighestBidOrderAsync(Settings.Symbol);
            _lowestAsk = await TradingClient.GetLowestAskAsync(Settings.Symbol);
        }

        private Task CancelOpenOrders()
        {
            return Task.CompletedTask;
        }

        private List<Order> CreateNewOrders()
        {
            var orders = new List<Order>();

            for (var i = 1; i <= Settings.OrderAmount; i++)
            {
                var buyPrice = decimal.Round(_highestBid * (1 - ((Settings.Difference * i) / 100)), DECIMALS);
                orders.Add(new Order
                {
                    Price = buyPrice,
                    Quantity = decimal.Round((Settings.Ordervalue / buyPrice), DECIMALS),
                    OrderSide = "buy",
                    Symbol = Settings.Symbol
                });

                var sellPrice = decimal.Round(_lowestAsk * (1 + ((Settings.Difference * i) / 100)), DECIMALS);
                orders.Add(new Order
                {
                    Price = sellPrice,
                    Quantity = decimal.Round((Settings.Ordervalue / sellPrice), DECIMALS),
                    OrderSide = "sell",
                    Symbol = Settings.Symbol
                });
            }

            return orders;
        }

        private Task ExecuteOrders(List<Order> orders)
        {
            return TradingClient.PlaceOrdersAsync(orders);
        }
    }
}
