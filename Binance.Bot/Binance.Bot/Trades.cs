namespace Binance.Bot
{
    public class Trades
    {
        //TODO persist to memory
        public decimal BuyAverage
        {
            get
            {
                if (_sumPriceBuy == 0)
                    return 0;
                return _sumPriceBuy / _totalTradesBuy;
            }
        }


        public decimal SellAverage
        {
            get
            {
                if (_sumPriceSell == 0)
                    return 0;
                return _sumPriceSell / _totalTradesSell;
            }
        }

        private decimal _sumPriceBuy=0;
        private decimal _sumPriceSell=0;
        private int _totalTradesBuy=0;
        private int _totalTradesSell=0;

        public void AddBuy(decimal price)
        {
            _sumPriceBuy += price;
            _totalTradesBuy++;
        }

        public void AddSell(decimal price)
        {
            _sumPriceSell += price;
            _totalTradesSell++;
        }




    }
}