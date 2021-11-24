namespace Binance.Bot
{
    public class BotSetting
    {
        public string Symbol { get; set; }
        public int TimeSpan { get; set; }
        public decimal ChangeInPriceUp { get; set; }
        public  decimal ChangeInPriceDown { get; set; }
        public  decimal QuantityInDollar { get; set; }
    }
}