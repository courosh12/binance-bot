using System;
using Trading.Bot.Enums;

namespace Trading.Bot.Data
{
    public class TradesEntity
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public OrderSide OrderType { get; set; }
        public DateTime ExecutionTime { get; set; }
        public int BotId { get; set; }
        public BotEntity Bot { get; set; }
    }
}