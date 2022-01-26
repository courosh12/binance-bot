using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trading.Bot.Models
{
    public class Order
    {
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public string Symbol { get; set; }
        public string OrderSide { get; set; }

    }
}
