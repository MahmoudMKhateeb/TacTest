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
        public string TruckTypeDisplayName { get; set; }
        public string GoodCategoryName { get; set; }
        public int BidsNo { get; set; }
        public double LastBidPrice { get; set; }
        public long FirstBidId { get; set; }
        public string OriginalCityName { get; set; }
        public string DestinationCityName { get; set; }

    }
}
