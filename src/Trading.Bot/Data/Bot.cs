using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Bot.Bots.MarketMaker;
using Trading.Bot.Enums;

namespace Trading.Bot.Data
{
    public class Bot
    {
        public int Id { get; set; }
        public BotName BotName { get; set; }
        public Exchange Exchange { get; set; }
        public string BotIdentifier { get; set; }
        public List<TradesEntity> Trades { get; set; }
    }
}
