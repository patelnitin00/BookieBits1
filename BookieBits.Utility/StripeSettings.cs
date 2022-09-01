using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookieBits.Utility
{
    public class StripeSettings
    {
        //the name must be same as in json file
        public string SecretKey { get; set; }
        public string PublishableKey { get; set; }
    }
}
