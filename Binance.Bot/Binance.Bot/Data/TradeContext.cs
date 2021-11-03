using Microsoft.EntityFrameworkCore;

namespace Binance.Bot.Data
{
    public class TradeContext: DbContext
    {
        public DbSet<Bot> Bots { get; set; }
        public DbSet<TradesEntity> Trades { get; set; }
        public TradeContext(DbContextOptions<TradeContext> options) : base(options)
        {
        }
    }   
}