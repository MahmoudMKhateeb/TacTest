using System.ComponentModel;

namespace TACHYON.Shipping.SrPostPriceUpdates
{
    public enum SrPostPriceUpdateOfferStatus
    {
        [Description("None")]
        None = 0,
        [Description("PendingShipperAction")]
        Pending = 1,
        [Description("OfferAccepted")]
        Accepted = 2,
        [Description("OfferRejected")]
        Rejected = 3
        
    }
}