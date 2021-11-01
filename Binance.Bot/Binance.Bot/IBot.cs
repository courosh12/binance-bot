namespace Binance.Bot
{
    public interface IBot
    {
        void SubscribeToData();
        void ShowAverageBuySell();
    }
}