using System;
using System.Collections.Generic;
using TACHYON.Routs.RoutPoints.Dtos;
using TACHYON.ShippingRequestVases.Dtos;

namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    public class GetShippingRequestForEditOutput
    {
        public CreateOrEditShippingRequestDto ShippingRequest { get; set; }
    }
}