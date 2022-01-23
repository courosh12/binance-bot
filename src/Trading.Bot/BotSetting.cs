using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Trading.Bot
{
    public class BotSetting
    {
        public string Name { get; set; }
        public Dictionary<string,string> Setting { get; set; }
    }
}