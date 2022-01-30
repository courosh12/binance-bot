using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trading.Bot.Data;
using Trading.Bot.Enums;
using Trading.Bot.Models;
using Trading.Bot.Repositories;
using Trading.Bot.ServerClients;

namespace Trading.Bot.Bots.MarketMaker
{
    public class MarketMaker : TradingBotBase<MarketMaker, MarketMakerOptions>
    {
        private decimal _lowestAsk;
        private decimal _highestBid;
        private List<long> _placedOrderIds;

        public MarketMaker(ILogger<MarketMaker> logger, TradeHistoryRepository tradeHistoryRepository)
            : base(logger, tradeHistoryRepository, BotName.MARKET_MAKER)
        {
        }

        protected override void SetBotIdentifier()
        {
            BotIdentifier = $"{Botname}.{BotOptions.Exchange}.{BotOptions.Symbol}.{BotOptions.Spread}.{BotOptions.Interval}." +
                $"{BotOptions.OrderQuantity}.{BotOptions.Ordervalue}";
        }

        protected override async Task ExecuteBotStepsAsync(CancellationToken cancelToken)
        {
            await GetPricesAsync();
            await CancelOpenOrdersAsync();
            await UpdateOrderHistory(_placedOrderIds);
            var orders = CreateNewOrders();
            _placedOrderIds = await ExecuteOrdersAsync(orders);
            await Task.Delay(1000 * 60 * BotOptions.Interval, cancelToken);
        }


        private async Task GetPricesAsync()
        {
            _highestBid = await TradingClient.GetHighestBidOrderAsync(BotOptions.Symbol);
            _lowestAsk = await TradingClient.GetLowestAskAsync(BotOptions.Symbol);
        }

        private Task CancelOpenOrdersAsync()
        {
            return TradingClient.CancelAllOrdersAsync(BotOptions.Symbol);
        }

        private async Task UpdateOrderHistory(List<long> placedOrderIds)
        {
            if (placedOrderIds == null || !placedOrderIds.Any())
                return;

            var trades = await TradingClient.GetExecutedOrdersAsync(BotOptions.Symbol, placedOrderIds);

            if (trades.Any())
                TradeHistoryRepository.UpdateTrades(trades, BotIdentifier);
        }

        private List<Order> CreateNewOrders()
        {
            var orders = new List<Order>();

            for (var i = 1; i <= BotOptions.OrderQuantity; i++)
            {
                var buyPrice = decimal.Round(_highestBid * (1 - ((BotOptions.Spread * i) / 100)), DECIMALS);
                orders.Add(new Order
                {
                    Price = buyPrice,
                    Quantity = decimal.Round((BotOptions.Ordervalue / buyPrice), DECIMALS),
                    OrderSide = OrderSide.BUY,
                    Symbol = BotOptions.Symbol
                });

                var sellPrice = decimal.Round(_lowestAsk * (1 + ((BotOptions.Spread * i) / 100)), DECIMALS);
                orders.Add(new Order
                {
                    Price = sellPrice,
                    Quantity = decimal.Round((BotOptions.Ordervalue / sellPrice), DECIMALS),
                    OrderSide = OrderSide.SELL,
                    Symbol = BotOptions.Symbol
                });
            }

            return orders;
        }

        private Task<List<long>> ExecuteOrdersAsync(List<Order> orders)
        {
            return TradingClient.PlaceOrdersAsync(orders);
        }

        protected override async Task ExitingBotAsync()
        {
            await CancelOpenOrdersAsync();
        }
    }
}
