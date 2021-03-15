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
        public double? MyBidPrice { get; set; }
        /// <summary>
        /// carrier bid Id if exist
        /// </summary>
        public long? MyBidId { get; set; }
        /// <summary>
        /// shipping request origin facility output
        /// </summary>
        public GetFacilityForViewOutput OriginalFacility { get; set; }
        /// <summary>
        /// shipping request destination facility output
        /// </summary>
        public GetFacilityForViewOutput DestinationFacility { get; set; }
        /// <summary>
        /// shipping request good category
        /// </summary>
        public string GoodCategoryName { get; set; }
        public string SourceCountryName { get; set; }
        public string SourceCityName { get; set; }
        public string DestinationCountryName { get; set; }
        public string DestinationCityName { get; set; }
        public IEnumerable<GetShippingRequestVasForViewDto> ShippingRequestVasesDto { get; set; }


    }
}
