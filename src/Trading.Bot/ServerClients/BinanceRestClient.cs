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
using Trading.Bot.Data;
using Trading.Bot.Enums;
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

        public Task<List<Order>> PlaceMarketOrdersAsync(List<Order> orders)
        {
            return PlaceOrdersAsync(orders, OrderType.Market);
        }

        public Task<List<Order>> PlaceLimitOrdersAsync(List<Order> orders)
        {
            return PlaceOrdersAsync(orders, OrderType.Limit);
        }

        private async Task<List<Order>> PlaceOrdersAsync(List<Order> orders, OrderType orderType)
        {
            var placedOrders = new List<Order>();
            foreach (var order in orders)
            {
                var orderResultCall = await _client.Spot.Order.PlaceOrderAsync(
                    order.Symbol,
                    order.OrderSide == Enums.OrderSide.BUY ? Binance.Net.Enums.OrderSide.Buy :
                        order.OrderSide == Enums.OrderSide.SELL ? Binance.Net.Enums.OrderSide.Sell :
                            throw new Exception("ordertype not supported"),
                    orderType,
                    quantity: order.Quantity,
                    price: orderType == OrderType.Limit ? order.Price:null,
                    timeInForce: orderType == OrderType.Limit ? TimeInForce.GoodTillCancel : null);

                if (ValidateResult(orderResultCall, order))
                {
                    order.OrderId = orderResultCall.Data.OrderId;
                    placedOrders.Add(order);
                }

                _logger.LogInformation($"Order placed: {order.OrderSide} {order.Symbol} {orderType} {order.Quantity} {order.Price}");
            }

            return placedOrders;
        }

        public async Task CancelAllOrdersAsync(string symbol)
        {
            var callResult = await _client.Spot.Order.CancelAllOpenOrdersAsync(symbol);
            ValidateResult(callResult, symbol);
        }

        public async Task<List<TradesEntity>> GetExecutedOrdersAsync(List<Order> orders)
        {
            var trades = new List<TradesEntity>();

            foreach (var orderToGet in orders)
            {
                var order = await _client.Spot.Order.GetOrderAsync(orderToGet.Symbol, orderToGet.OrderId);
                ValidateResult(order);

                if (order.Data.Status == OrderStatus.Filled) //skip partially filled for now
                {
                    trades.Add(new TradesEntity()
                    {
                        OrderType = order.Data.Side == Binance.Net.Enums.OrderSide.Buy ? Bot.Enums.OrderSide.BUY : Bot.Enums.OrderSide.SELL,
                        Price = order.Data.Price,
                        Quantity = order.Data.Quantity,
                        ExecutionTime = order.Data.UpdateTime.HasValue ? order.Data.UpdateTime.Value : order.Data.CreateTime
                    }); ;
                }
            }

            return trades;
        }

        public async Task<List<AssetBalance>> GetAllBalances()
        {
            var accountResult = await _client.General.GetAccountInfoAsync();
            ValidateResult(accountResult);
            return accountResult.Data.Balances.Select(p => new AssetBalance() { Name = p.Asset, Value = p.Free }).ToList();
        }

        private bool ValidateResult<T>(WebCallResult<T> result, object argument = null)
        {
            if (result.Success)
            {
                return true;
            }
            else if (result.Error.Code == -2010)
            {
                var order = argument as Order;
                _logger.LogError($"Not enough balance to {order.OrderSide} {order.Symbol}");
                return false;
            }
            else if (result.Error.Code == -2011)
            {
                var symbol = argument as string;
                _logger.LogError($"No orders to cancel for {symbol}");
                return false;
            }
            else
            {
                throw new Exception($"Call went wrong: {result.ResponseStatusCode} message: {result.Error}");
            }
        }
    }
}
