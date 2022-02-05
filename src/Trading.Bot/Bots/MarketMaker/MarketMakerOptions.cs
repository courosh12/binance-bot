using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Bot.Enums;

namespace Trading.Bot.Bots.MarketMaker
{
    public class MarketMakerOptions
    {
        public string Symbol { get; set; }
        public decimal Spread { get; set; }
        public decimal Ordervalue { get; set; }
        public int OrderQuantity { get; set; }
        public int Interval { get; set; }
        public Exchange Exchange { get; set; } = Exchange.BINANCE;
    }
}
