using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequestBids.Dtos
{
    public class ViewShipperBidsReqDetailsOutput
    {
        public long Id { get; set; }
        public DateTime? StartBidDate { get; set; }
        public DateTime? EndBidDate { get; set; }
        public bool IsOngoingBid { get; set; }
        public string ShipperName { get; set; }
        public string ShippingRequestBidStatusName { get; set; }

    }
}
