using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Bot.Models;

namespace Trading.Bot.ServerClients
{
    public interface ITradingClient
    {
        Task<decimal> GetHighestBidOrderAsync(string symbol);
        Task<decimal> GetLowestAskAsync(string symbol);
        Task PlaceOrdersAsync(List<Order> orders);
    }
}
