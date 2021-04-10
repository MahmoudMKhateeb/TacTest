using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.Trips.RejectReasons.Dtos
{
    public class ShippingRequestTripRejectReasonListDto : EntityDto
    {
        public string DisplayName { get; set; }
    }
}
