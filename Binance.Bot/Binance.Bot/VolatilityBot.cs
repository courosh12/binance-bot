using System;
using Binance.Bot.Data;
using Binance.Net;
using Binance.Net.Enums;
using Binance.Net.Interfaces;
using Binance.Net.Objects.Spot.MarketStream;
using CryptoExchange.Net;
using CryptoExchange.Net.Sockets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SQLitePCL;
using ILogger = Serilog.ILogger;

namespace Binance.Bot
{
    public class VolatilityBot:IBot
    {
        private BinanceSocketClient _socketClient;
        private BinanceClient _client;
        private  ILogger<VolatilityBot> _logger;
        private BotSetting _botSetting;
        private RollingStack<Trade> _stack;
        private decimal _currentPrice;
        private DateTime _dontTradeTill;
        private object _lock = new object();
        private TradesService _tradesService;
        private Action _newTradeCallback;

        public VolatilityBot(BinanceSocketClient socketClient, BinanceClient client, 
            ILogger<VolatilityBot> logger, BotSetting botSetting, Action tradeCallback,TradesService tradesService)
        {
            _socketClient = socketClient;
            _client = client;
            _logger = logger;
            _botSetting = botSetting;
            _newTradeCallback = tradeCallback;
            _stack = new RollingStack<Trade>(_botSetting.TimeSpan);
            _tradesService = tradesService;
            _tradesService.BotSetting = _botSetting;
            _tradesService.SetId();
            _logger.LogInformation($"Starting: {this.GetType().Name} on Pair {_botSetting.Symbol} " +
                                   $"setting: Timespan: {botSetting.TimeSpan} ChangeInPrice: {botSetting.ChangeInPrice}");
        }

        public void SubscribeToData()
        {
            var subscribeResult = _socketClient.Spot.SubscribeToKlineUpdatesAsync(_botSetting.Symbol,KlineInterval.OneMinute, OnKlineUpdate);
            
            if (!subscribeResult.Result.Success)
            {
                _logger.LogError(subscribeResult.Result.Error.Message);
            }
            
            var subscribeResultPrice = _socketClient.Spot.SubscribeToTradeUpdatesAsync(_botSetting.Symbol, OnTradeUpdate);
            
            if (!subscribeResultPrice.Result.Success)
            {
                _logger.LogError(subscribeResultPrice.Result.Error.Message);
            }
        }

        private void OnKlineUpdate(DataEvent<IBinanceStreamKlineData> data)
        {
            var actualData = data.Data.Data;
            if (actualData.Final)
            {
                _stack.Push(new Trade(){Price = actualData.Close});
                _logger.LogDebug($"{_botSetting.Symbol} close{actualData.Close.ToString()} time: {actualData.CloseTime} ");
            }
        }

        private void OnTradeUpdate(DataEvent<BinanceStreamTrade> trade)
        {
            if (_dontTradeTill != null && _dontTradeTill > DateTime.Now)
            {
                return;
            }
            
            _currentPrice = trade.Data.Price;
            var action = CheckpriceDifference(_currentPrice);
            
            if(action==TypeOfTrade.Buy)
            {
                ExceCuteOrder(OrderSide.Buy);    
            }
            else if (action == TypeOfTrade.Sell)
            {
                ExceCuteOrder(OrderSide.Sell);
            }
        }

        private void ExceCuteOrder(OrderSide type)
        {
            lock (_lock)
            {
                if (_dontTradeTill != null && _dontTradeTill > DateTime.Now)
                {
                    return;
                }
                
                var quantity = _botSetting.QuantityInDollar/_currentPrice;

                var callResult = _client.Spot.Order.PlaceOrderAsync
                    (_botSetting.Symbol, type, OrderType.Market, quantity: quantity).GetAwaiter().GetResult();

                if(callResult.Success)
                {
                    var actualData=callResult.Data;
                    
                    if(type == OrderSide.Buy)
                        _tradesService.AddTrade(actualData.Price,decimal.Round(actualData.Quantity*actualData.Price,2),TypeOfTrade.Buy);
                    else if(type==OrderSide.Sell)
                        _tradesService.AddTrade(actualData.Price,decimal.Round(actualData.Quantity*actualData.Price,2),TypeOfTrade.Sell);
                    
                    _logger.LogInformation($"{(type == OrderSide.Buy?"Bought":"Sold")}: {actualData.Quantity} of {actualData.Symbol} at {actualData.Price} price");
                    _stack.SetHistoryTo(new Trade(){Price = actualData.Price});
                    _newTradeCallback();
                }
                else
                {
                    _logger.LogError(callResult.Error.ToString());
                    if (callResult.Error.Code == -2010)
                    {
                        _dontTradeTill = DateTime.Now.AddMinutes(_botSetting.TimeSpan);
                        _logger.LogInformation($"Cant trade till {_dontTradeTill}");
                    }
                }
            }
        }
        
        public void ShowAverageBuySell()
        {
            var buyRapport = _tradesService.GetTradeRapport(TypeOfTrade.Buy);
            var sellRapport=_tradesService.GetTradeRapport(TypeOfTrade.Sell);;
            _logger.LogInformation(
                 $"Symbol: {_botSetting.Symbol}" +
                        $" average buy: {buyRapport.AveragePrice} trades: {buyRapport.TotalTrades} amount: {buyRapport.SumQauntity} " +
                        $" average sell: {sellRapport.AveragePrice} trades: {sellRapport.TotalTrades} amount: {sellRapport.SumQauntity} " +
                        $"diff: {decimal.Round(buyRapport.AveragePrice>0?(sellRapport.AveragePrice-buyRapport.AveragePrice)/buyRapport.AveragePrice*100:0,2)} ");
        }

        private TypeOfTrade CheckpriceDifference(decimal price)
        {
            if (_stack.LookUp(_botSetting.TimeSpan) == null)
                return TypeOfTrade.None;
            
            var prevPrice = _stack.LookUp(_botSetting.TimeSpan).Price;
            var priceChange = ((price - prevPrice) / prevPrice) * 100;
            _logger.LogDebug($"Symbol: {_botSetting.Symbol} Price Change: {prevPrice} > {price} percentage: {priceChange}");

            if (Math.Abs(priceChange) > _botSetting.ChangeInPrice)
            {
                _logger.LogInformation($"Price Change: {prevPrice} > {price} percentage: {priceChange}");

                if (priceChange > 0)
                    return TypeOfTrade.Sell;
                else
                    return TypeOfTrade.Buy;
            }

            return TypeOfTrade.None;
        }
    }
}