using Abp.Application.Services.Dto;
using System;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.Trips;

namespace TACHYON.Shipping.Drivers.Dto
{
    public class ShippingRequestTripDriverListDto : EntityDto<long>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ShippingRequestRouteType RouteTypeId { get; set; }
        public string Source { get; set; }
        public string Distination { get; set; }
        public ShippingRequestTripDriverLoadStatusDto DriverLoadStatus { get; set; }

        public ShippingRequestTripStatus Status { get; set; }
        public string StatusTitle { get; set; }
        public string TripStatusTitle { get; set; }
        public ShippingRequestTripDriverStatus DriverStatus { get; set; }
        public long? WaybillNumber { get; set; }
        public bool HasIncident { get; set; }
        public bool IsSaas { get; set; }
    }
}