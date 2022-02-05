using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Bot.Data;
using Trading.Bot.Models;

namespace Trading.Bot.ServerClients
{
    public interface ITradingClient
    {
        Task<decimal> GetHighestBidOrderAsync(string symbol);
        Task<decimal> GetLowestAskAsync(string symbol);
        Task<List<Order>> PlaceLimitOrdersAsync(List<Order> orders);
        Task<List<Order>> PlaceMarketOrdersAsync(List<Order> orders);
        Task CancelAllOrdersAsync(string symbol);
        Task<List<AssetBalance>> GetAllBalances();
        Task<List<TradesEntity>> GetExecutedOrdersAsync(List<Order> orders);
    }
}
