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
        [Description("Rejected")] /// If the status is force rejected the carrier can not Re-pricing  offer 
        ForceRejected = 5,
        /// Accepted By TAD by TAD still not send offer to shipper when TAD send offer then check of there pending offer then change to AcceptedAndWaitingForShipper and add parent id to current offer created
        Pending = 6

    }
}
