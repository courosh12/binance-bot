using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trading.Bot.Enums;
using Trading.Bot.Models;
using Trading.Bot.Repositories;
using Trading.Bot.ServerClients;

namespace Trading.Bot.Bots.MarketMaker
{
    public class MarketMaker : ITradingBot
    {
        public ITradingClient TradingClient { get; set; }
        public MarketMakerOptions Settings { get; set; }
        public TradeHistoryRepository TradeHistoryRepository { get; set; }

        private readonly ILogger _logger;
        private decimal _lowestAsk;
        private decimal _highestBid;
        private const int DECIMALS = 8;
        private string _botIdentifier;

        public MarketMaker(ILogger<MarketMaker> logger)
        {
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancelToken)
        {
            _botIdentifier = nameof(MarketMaker) + Settings.Symbol;
            _logger.LogInformation($"Starting bot: {_botIdentifier}");

            List<long> placedOrderIds = null;
            while (!cancelToken.IsCancellationRequested)
            {
                try
                {
                    await GetPricesAsync();
                    await CancelOpenOrdersAsync();
                    await UpdateOrderHistory(placedOrderIds);
                    var orders = CreateNewOrders();
                    placedOrderIds = await ExecuteOrdersAsync(orders);
                    await Task.Delay(1000 * 60 * Settings.Delay, cancelToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Something went wrong for {_botIdentifier + Environment.NewLine + ex.Message} ");
                }
            }

            await CancelOpenOrdersAsync();
            _logger.LogInformation($"Exiting bot: {_botIdentifier}");
        }

        private async Task GetPricesAsync()
        {
            _highestBid = await TradingClient.GetHighestBidOrderAsync(Settings.Symbol);
            _lowestAsk = await TradingClient.GetLowestAskAsync(Settings.Symbol);
        }

        private Task CancelOpenOrdersAsync()
        {
            return TradingClient.CancelAllOrdersAsync(Settings.Symbol);
        }

        private async Task UpdateOrderHistory(List<long> placedOrderIds)
        {
            if (placedOrderIds == null || !placedOrderIds.Any())
                return;

            var trades = await TradingClient.GetExecutedOrdersAsync(Settings.Symbol, placedOrderIds);
            TradeHistoryRepository.UpdateTrades(trades);
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
                    OrderSide = OrderSide.BUY,
                    Symbol = Settings.Symbol
                });

                var sellPrice = decimal.Round(_lowestAsk * (1 + ((Settings.Difference * i) / 100)), DECIMALS);
                orders.Add(new Order
                {
                    Price = sellPrice,
                    Quantity = decimal.Round((Settings.Ordervalue / sellPrice), DECIMALS),
                    OrderSide = OrderSide.SELL,
                    Symbol = Settings.Symbol
                });
            }

            return orders;
        }

        private Task<List<long>> ExecuteOrdersAsync(List<Order> orders)
        {
            return TradingClient.PlaceOrdersAsync(orders);
        }
    }
}
