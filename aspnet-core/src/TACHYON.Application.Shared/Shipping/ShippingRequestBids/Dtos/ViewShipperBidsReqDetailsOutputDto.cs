using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequestBids.Dtos
{
    public class GetAllBidShippingRequestsForCarrierOutput
    {
        public long ShippingRequestId { get; set; }
        /// <summary>
        /// ShippingRequest bidding startDate
        /// </summary>
        public DateTime? BidStartDate { get; set; }
        /// <summary>
        /// ShippingRequest bidding endDate
        /// </summary>
        public DateTime? BidEndDate { get; set; }
        /// <summary>
        /// tenant name who created the shipping-request
        /// </summary>
        public string ShipperName { get; set; }
        public string ShippingRequestBidStatusName { get; set; }
        public string TruckTypeDisplayName { get; set; }
        /// <summary>
        /// carrier bid price
        /// </summary>
        public double? MyBidPrice { get; set; }
        /// <summary>
        /// carrier bid Id if exist
        /// </summary>
        public long? MyBidId { get; set; }
        /// <summary>
        /// shipping request origin city name
        /// </summary>
        public string OriginalCityName { get; set; }
        /// <summary>
        /// shipping request destination city name
        /// </summary>
        public string DestinationCityName { get; set; }
        /// <summary>
        /// shipping request good category
        /// </summary>
        public string GoodCategoryName { get; set; }


    }
}
