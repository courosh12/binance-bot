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

namespace Trading.Bot.Bots
{
    public abstract class TradingBotBase<TBot, TBotOptions> : ITradingBot<TBotOptions>
    {
        protected string BotIdentifier;
        protected readonly BotName Botname;
        protected readonly TradeHistoryRepository TradeHistoryRepository;
        protected readonly ILogger Logger;
        protected const int DECIMALS = 8;

        public TBotOptions BotOptions { get; set; }
        public ITradingClient TradingClient { get; set; }

        protected TradingBotBase(ILogger<TBot> logger, TradeHistoryRepository tradeHistoryRepository, BotName botname)
        {
            TradeHistoryRepository = tradeHistoryRepository;
            Logger = logger;
            Botname = botname;
        }

        public async Task StartAsync(CancellationToken cancelToken)
        {
            try
            {
                StartingUpBot();
                Logger.LogInformation($"Starting bot: {BotIdentifier}");
                AddBotToDb();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Something went wrong setting up bot {ex.Message + Environment.NewLine + ex.StackTrace} ");
                throw ex;
            }


            while (!cancelToken.IsCancellationRequested)
            {
                try
                {
                    await ExecuteBotStepsAsync(cancelToken);
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Something went wrong during bot execution " +
                        $"{BotIdentifier + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace} ");
                    await Task.Delay(1000 * 60 * 10, cancelToken);
                }
            }

            await ExitingBotAsync();
            Logger.LogInformation($"Exiting bot: {BotIdentifier}");
        }

        protected async Task UpdateOrderHistory(List<Order> placedOrders)
        {
            if (placedOrders == null || !placedOrders.Any())
                return;

            var trades = await TradingClient.GetExecutedOrdersAsync(placedOrders);

            if (trades.Any())
                TradeHistoryRepository.UpdateTrades(trades, BotIdentifier);
        }

        private void AddBotToDb()
        {
            var bot = new BotEntity()
            {
                BotIdentifier = BotIdentifier,
            };
            TradeHistoryRepository.AddBotToDb(bot);
        }

        protected abstract void StartingUpBot();
        protected abstract Task ExecuteBotStepsAsync(CancellationToken canceltToken);
        protected abstract Task ExitingBotAsync();
    }
}
