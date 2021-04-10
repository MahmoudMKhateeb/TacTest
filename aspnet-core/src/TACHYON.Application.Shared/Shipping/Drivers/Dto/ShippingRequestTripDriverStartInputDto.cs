using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.Drivers.Dto
{
    public class ShippingRequestTripDriverStartInputDto : EntityDto
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }
}
