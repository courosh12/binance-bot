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
    public class MarketMaker : ITradingBot<MarketMakerOptions>
    {
        public ITradingClient TradingClient { get; set; }
        public MarketMakerOptions BotOptions { get; set; }

        private string _botIdentifier;
        private BotName _botName = BotName.MARKET_MAKER;
        private TradeHistoryRepository _tradeHistoryRepository;
        private readonly ILogger _logger;
        private decimal _lowestAsk;
        private decimal _highestBid;
        private const int DECIMALS = 8;

        public MarketMaker(ILogger<MarketMaker> logger, TradeHistoryRepository tradeHistoryRepository)
        {
            _tradeHistoryRepository = tradeHistoryRepository;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancelToken)
        {
            _logger.LogInformation($"Starting bot: {_botIdentifier}");

            _botIdentifier = $"{_botName}.{BotOptions.Exchange}.{BotOptions.Symbol}.{BotOptions.Spread}.{BotOptions.Interval}." +
                $"{BotOptions.OrderQuantity}.{BotOptions.Ordervalue}";

            AddBotToDb();

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
                    await Task.Delay(1000 * 60 * BotOptions.Interval, cancelToken);
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
            trades = new List<TradesEntity>()
            {
                new TradesEntity()
                {
                    ExecutionTime = DateTime.Now,
                    OrderType=OrderSide.SELL,
                    Price=2m,
                    Quantity=1,

                }
            };

            if (trades.Any())
                _tradeHistoryRepository.UpdateTrades(trades, _botIdentifier);
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

        private void AddBotToDb()
        {
            var bot = new BotEntity()
            {
                BotIdentifier = _botIdentifier,
            };

            _tradeHistoryRepository.AddBotToDb(bot);
        }
    }
}
