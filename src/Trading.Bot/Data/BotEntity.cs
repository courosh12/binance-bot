using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Bot.Bots.MarketMaker;
using Trading.Bot.Enums;

namespace Trading.Bot.Data
{
    public class BotEntity
    {
        public int Id { get; set; }
        public string BotIdentifier { get; set; }
        public List<TradesEntity> Trades { get; set; }
    }
}
