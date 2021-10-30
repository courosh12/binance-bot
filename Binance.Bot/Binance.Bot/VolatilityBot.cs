using System;
using Binance.Net;
using Binance.Net.Enums;
using Binance.Net.Interfaces;
using Binance.Net.Objects.Spot.MarketStream;
using CryptoExchange.Net;
using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Logging;
using ILogger = Serilog.ILogger;

namespace Binance.Bot
{
    public class VolatilityBot:IBot
    {
        private BinanceSocketClient _socketClient;
        private BinanceClient _client;
        private  ILogger<VolatilityBot> _logger;
        private BotSetting _botSetting;
        private RollingStack<IBinanceStreamKline> _stack;

        public VolatilityBot(BinanceSocketClient socketClient, BinanceClient client, 
            ILogger<VolatilityBot> logger, BotSetting botSetting)
        {
            _socketClient = socketClient;
            _client = client;
            _logger = logger;
            _botSetting = botSetting;
            _stack = new RollingStack<IBinanceStreamKline>();
        }

        public void SubscribeToData()
        {
            //should stop by gc??
            var subscribeResult = _socketClient.Spot.SubscribeToKlineUpdatesAsync(_botSetting.Symbol,KlineInterval.OneMinute, OnUpdate);
            
            if (!subscribeResult.Result.Success)
            {
                _logger.LogError(subscribeResult.Result.Error.Message);
            }
        }

        private void OnUpdate(DataEvent<IBinanceStreamKlineData> data)
        {
            var actualData = data.Data.Data;
            if (actualData.Final)
            {
                _stack.Push(actualData);
                _logger.LogInformation(actualData.Close.ToString());   
            }
        }

    }
}