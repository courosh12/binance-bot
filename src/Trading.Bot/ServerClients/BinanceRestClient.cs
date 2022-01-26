using Binance.Net;
using Binance.Net.Enums;
using Binance.Net.Objects.Spot.SpotData;
using CryptoExchange.Net.Objects;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Bot.Exceptions;
using Trading.Bot.Models;

namespace Trading.Bot.ServerClients
{
    internal class BinanceRestClient : ITradingClient
    {

        private readonly BinanceClient _client;
        private readonly ILogger _logger;

        public BinanceRestClient(BinanceClient binanceClient, ILogger<BinanceRestClient> logger)
        {
            _client = binanceClient;
            _logger = logger;
        }

        public async Task<decimal> GetHighestBidOrderAsync(string symbol)
        {
            var orderBookCall = await _client.Spot.Market.GetOrderBookAsync(symbol, 5);
            ValidateResult(orderBookCall);
            return orderBookCall.Data.Bids.OrderByDescending(p => p.Price).First().Price;
        }

        public async Task<decimal> GetLowestAskAsync(string symbol)
        {
            var orderBookCall = await _client.Spot.Market.GetOrderBookAsync(symbol, 5);
            ValidateResult(orderBookCall);
            return orderBookCall.Data.Asks.OrderBy(p => p.Price).First().Price;
        }

        public async Task PlaceOrdersAsync(List<Order> orders)
        {

            foreach (var order in orders)
            {
                var orderResultCall = await _client.Spot.Order.PlaceOrderAsync(
                    order.Symbol,
                    order.OrderSide == "buy" ? OrderSide.Buy : order.OrderSide == "sell" ? OrderSide.Sell : throw new Exception("ordertype not supported"),
                    OrderType.Limit,
                    quantity: order.Quantity,
                    price: order.Price,
                    timeInForce: TimeInForce.GoodTillCancel);

                ValidateResult(orderResultCall, order);
            }
        }

        public void ValidateResult<T>(WebCallResult<T> result, object argument = null)
        {
            if (result.Success)
            {
                return;
            }
            else if (result.Error.Code == -2010)
            {
                var order = argument as Order;
                _logger.LogError($"Not enough balance to {order.OrderSide} {order.Symbol}");
            }
            else
            {
                throw new Exception($"Call went wrong: {result.ResponseStatusCode} message: {result.Error}");
            }
        }
    }
}
