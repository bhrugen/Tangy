using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tangy.Models.OrderDetailsViewModel
{
    public class OrderDetailsCart
    {
        public List<ShoppingCart> listCart { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}
