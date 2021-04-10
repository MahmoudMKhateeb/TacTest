using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.TachyonPriceOffers
{
    public enum OfferStatus:byte
    {
        Pending=0, //By default, not accepted nor rejected
        Accepted=1, //accepted and there is assigned carrier
        Rejected=2,
        AcceptedAndWaitingForCarrier=3
    }
}
