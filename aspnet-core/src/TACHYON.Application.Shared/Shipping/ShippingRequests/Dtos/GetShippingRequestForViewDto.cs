using System.Collections.Generic;
using TACHYON.Shipping.ShippingRequestBids.Dtos;

namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    public class GetShippingRequestForViewDto
    {
        public ShippingRequestDto ShippingRequest { get; set; }

        public List<ShippingRequestBidsDto> ShippingRequestBidDtoList { get; set; }


    }
}