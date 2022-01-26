using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trading.Bot.Exceptions
{
    public class LowBalanceException : Exception
    {
        public string Symbol { get; }

        public LowBalanceException(string symbol)
        {
            Symbol = symbol;
        }
    }
}
