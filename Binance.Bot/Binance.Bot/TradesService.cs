using System;
using System.Linq;
using Binance.Bot.Data;
using Microsoft.EntityFrameworkCore;

namespace Binance.Bot
{
    public class TradesService
    {
        public BotSetting BotSetting { get; set; }
        private IDbContextFactory<TradeContext> _trcontext;
        private int _botId;//
        public TradesService(IDbContextFactory<TradeContext> trContext)
        {
            _trcontext = trContext;
        }

        public void SetId()
        {
            using (var context = _trcontext.CreateDbContext())
            {
                _botId=context.Bots.First(p =>
                    p.Symbol == BotSetting.Symbol && p.TimeSpan == BotSetting.TimeSpan &&
                    p.ChangeInPriceUp == BotSetting.ChangeInPriceUp &&
                    p.ChangeInPriceDown==BotSetting.ChangeInPriceDown).Id;
            }
        }
        public TradeRapport GetTradeRapport(TypeOfTrade type)
        {
            Data.Bot bot;
            using (var context = _trcontext.CreateDbContext())
            {
                bot = context.Bots
                    .Include(p=>p.Trades.Where(p=>p.TypeOfTrade==type))
                    .First(p=>p.Id==_botId);
            }

            return new TradeRapport()
            {
                AveragePrice = (bot.Trades.Any() ? bot.Trades.Sum(p => p.Price) / bot.Trades.Count() : 0),
                TotalTrades = bot.Trades.Count(),
                SumQauntity = bot.Trades.Sum(p=>p.Quantity)
            };
        }

        public void AddTrade(decimal price,decimal quantity,TypeOfTrade type)
        {
            using (var context = _trcontext.CreateDbContext())
            {
                var trade = new TradesEntity()
                {
                    Price = price,
                    Quantity = quantity,
                    TypeOfTrade =type,
                    BotId = _botId
                };

                context.Trades.Add(trade);
                context.SaveChanges();
            }
        }
    }
}