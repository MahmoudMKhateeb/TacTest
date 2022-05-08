using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.TachyonPriceOffers
{
    public enum CarrierPriceType : byte
    {
        NotSet = 0,
        TachyonDealerBidding = 1,
        TachyonDirectRequest = 2,
        ShipperBidding = 3,
        ShipperDirectRequest = 4
    }
}