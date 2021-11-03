using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Binance.Bot.Data
{
    public class DesignTimeTradeContextFactory:IDesignTimeDbContextFactory<TradeContext>
    {
        public TradeContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<TradeContext>();
            options.UseSqlite();
            return new TradeContext(options.Options);
        }
    }
}