namespace Binance.Bot.Data
{
    public class TradesEntity
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public TypeOfTrade TypeOfTrade { get; set; }
        public int BotId { get; set; }
        public Bot Bot { get; set; }
    }
}