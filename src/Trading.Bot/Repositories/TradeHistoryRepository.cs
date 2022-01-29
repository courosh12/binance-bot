using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Bot.Data;

namespace Trading.Bot.Repositories
{
    public class TradeHistoryRepository
    {
        private IDbContextFactory<TradeContext> _tradeDbContextFactory;

        public TradeHistoryRepository(IDbContextFactory<TradeContext> dbContextFactory)
        {
            _tradeDbContextFactory = dbContextFactory;
        }

        public void AddBotToDb(BotEntity bot)
        {
            using var tradeContext = _tradeDbContextFactory.CreateDbContext();

            if (tradeContext.Bots.Where(p => p.BotIdentifier == bot.BotIdentifier).Any())
                return;

            tradeContext.Bots.Add(bot);
            tradeContext.SaveChanges();
        }

        public void UpdateTrades(List<TradesEntity> trades, string botIdentifier)
        {
            using var context = _tradeDbContextFactory.CreateDbContext();
            var bot = context.Bots.Where(p => p.BotIdentifier == botIdentifier).Single();
            trades.ForEach(p => p.BotId = bot.Id);
            context.Trades.AddRange(trades);
            context.SaveChanges();
        }
    }
}
