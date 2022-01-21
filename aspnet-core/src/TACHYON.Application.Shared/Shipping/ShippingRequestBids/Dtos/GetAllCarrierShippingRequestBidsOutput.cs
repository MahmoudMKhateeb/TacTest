using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Shipping.ShippingRequests.Dtos;
using TACHYON.Shipping.ShippingRequestStatuses;

namespace TACHYON.Shipping.ShippingRequestBids.Dtos
{
    public class GetAllCarrierShippingRequestBidsOutput
    {
        public ShippingRequestBidDto ShippingRequestBidDto { get; set; }
        public ShippingRequestDto ShippingRequestDto { get; set; }
        public string ShipperTenancyName { get; set; }
    }
}