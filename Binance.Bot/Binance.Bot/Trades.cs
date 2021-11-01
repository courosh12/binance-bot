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
                return _sumPriceBuy / TotalTradesBuy;
            }
        }


        public decimal SellAverage
        {
            get
            {
                if (_sumPriceSell == 0)
                    return 0;
                return _sumPriceSell / TotalTradesSell;
            }
        }

        private decimal _sumPriceBuy=0;
        private decimal _sumPriceSell=0;

        public int TotalTradesBuy { get; set; } = 0;
        public int TotalTradesSell { get; set; } = 0;

        public void AddBuy(decimal price)
        {
            _sumPriceBuy += price;
            TotalTradesBuy++;
        }

        public void AddSell(decimal price)
        {
            _sumPriceSell += price;
            TotalTradesSell++;
        }




    }
}