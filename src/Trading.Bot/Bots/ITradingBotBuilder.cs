using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Bot.Bots;

namespace Trading.Bot.Bots
{
    internal interface ITradingBotBuilder
    {
        void Reset();
        void SetServerClient();
        void SetBotOptions(Dictionary<string, string> setting);
        ITradingBot GetBot();
    }
}
