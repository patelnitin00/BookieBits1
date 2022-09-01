using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookieBits.Models.ViewModel
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> ListCart { get; set; }

       // public double CartTotal { get; set; }
       //cart total is available in order header so we can use that directly

        public OrderHeader OrderHeader { get; set; }
    }
}
