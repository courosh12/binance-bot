using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trading.Bot.Bots.MarketMaker
{
    public class MarketMakerOptions
    {
        public string Symbol { get; set; }
        public decimal Difference { get; set; }
        public decimal Ordervalue { get; set; }
        public int OrderAmount { get; set; }
    }
}
