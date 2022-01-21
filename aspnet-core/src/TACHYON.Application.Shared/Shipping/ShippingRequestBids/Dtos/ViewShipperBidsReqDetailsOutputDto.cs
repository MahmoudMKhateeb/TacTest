using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.AddressBook.Dtos;
using TACHYON.ShippingRequestVases.Dtos;

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
        /// tenant name who created the shipping-request
        /// </summary>
        public string ShipperName { get; set; }

        public string ShippingRequestBidStatusName { get; set; }
        public string TruckTypeDisplayName { get; set; }

        /// <summary>
        /// carrier bid price
        /// </summary>
        public decimal? MyBidPrice { get; set; }

        /// <summary>
        /// carrier bid Id if exist
        /// </summary>
        public long? MyBidId { get; set; }

        /// <summary>
        /// shipping request good category
        /// </summary>
        public string GoodCategoryName { get; set; }

        public int NumberOfTrips { get; set; }
        public double TotalWeight { get; set; }
        public string SourceCityName { get; set; }
        public string DestinationCityName { get; set; }
        public IEnumerable<GetShippingRequestVasForViewDto> ShippingRequestVasesDto { get; set; }
        public bool IsTachyonDeal { get; set; }
        public int TotalBids { get; set; }
    }
}