using System.ComponentModel;

namespace TACHYON.PriceOffers
{
    public  enum PriceOfferStatus:byte
    {
        New = 0,
        Accepted = 1, //accepted and to assigned carrier
        Rejected = 2,
        AcceptedAndWaitingForCarrier = 3,
        AcceptedAndWaitingForShipper = 4,
        [Description("Rejected")]
        ForceRejected =5,

    }
}
