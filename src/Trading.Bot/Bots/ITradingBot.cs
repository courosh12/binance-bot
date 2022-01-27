using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trading.Bot.ServerClients;

namespace Trading.Bot.Bots
{
    public interface ITradingBot
    {
        ITradingClient TradingClient { get; set; }
        Task StartAsync(CancellationToken token);
    }
}
