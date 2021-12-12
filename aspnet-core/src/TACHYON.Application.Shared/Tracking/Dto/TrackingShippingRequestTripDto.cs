using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Shipping.Drivers.Dto;
using TACHYON.Shipping.Trips;

namespace TACHYON.Tracking.Dto
{
    public class TrackingShippingRequestTripDto : EntityDto
    {
        public ShippingRequestTripDriverStatus DriverStatus { get; set; }
        public ShippingRequestTripStatus Status { get; set; }
        public List<ShippingRequestTripDriverRoutePointDto> RoutPoints { get; set; }
        public bool CanStartTrip { get; set; }
    }
}