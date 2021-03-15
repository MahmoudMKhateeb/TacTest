using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Shipping.Trips;
using TACHYON.Shipping.TripStatuses;

namespace TACHYON.Shipping.Drivers.Dto
{
 public  class ShippingRequestTripDriverListDto:EntityDto<long>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? RoutTypeId { get; set; }
        public string Source { get; set; }
        public string Distination { get; set; }
        public ShippingRequestTripStatus Status { get; set; }


    }
}
