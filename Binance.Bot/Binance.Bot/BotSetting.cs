namespace Binance.Bot
{
    public class BotSetting
    {
        public string Symbol { get; set; }
        public int TimeSpan { get; set; }
        public decimal ChangeInPrice { get; set; }
        public  decimal QuantityInDollar { get; set; }
    }
}