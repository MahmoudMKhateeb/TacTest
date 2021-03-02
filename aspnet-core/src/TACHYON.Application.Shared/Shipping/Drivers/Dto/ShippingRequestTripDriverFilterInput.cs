using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Shipping.TripStatuses;

namespace TACHYON.Shipping.Drivers.Dto
{
    public class ShippingRequestTripDriverFilterInput : PagedAndSortedResultRequestDto
    {
        public ShippingRequestTripStatus? Status { get; set; }
    }
}
