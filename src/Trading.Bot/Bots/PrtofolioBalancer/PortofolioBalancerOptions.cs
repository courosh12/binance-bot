using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Bot.Enums;
using Trading.Bot.Models;

namespace Trading.Bot.Bots.PrtofolioBalancer
{
    public class PortofolioBalancerOptions
    {
        public string Market { get; set; }
        public List<AssetAllocation> Allocation { get; set; }
        public decimal Trigger { get; set; }
        public int Interval { get; set; }
        public Exchange Exchange { get; set; } = Exchange.BINANCE;

    }
}
