using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.TachyonPriceOffers
{
    public enum PriceType:byte
    {
        GuesingPrice = 1,// expected price by tachyon user
        Bidding = 2,
        DirectRequest=3 // price comes from direct request to carrier
    }
}
