using System.Collections.Generic;

namespace Binance.Bot.Data
{
    public class Bot
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public int TimeSpan { get; set; }
        public decimal ChangeInPriceUp { get; set; }
        public  decimal ChangeInPriceDown { get; set; }
        public List<TradesEntity> Trades { get; set; }
    }
}