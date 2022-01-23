using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trading.Bot.ServerClients
{
    public interface ITradingClient
    {
        Task<decimal> GetHighestBidOrderAsync(string symbol);
        Task<decimal> GetHighestAskOrderAsync(string symbol);
    }
}
