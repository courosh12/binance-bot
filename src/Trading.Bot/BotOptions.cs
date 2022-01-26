using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Trading.Bot
{
    public class BotOptions
    {
        public string Name { get; set; }
        public Dictionary<string,string> Setting { get; set; }
    }
}