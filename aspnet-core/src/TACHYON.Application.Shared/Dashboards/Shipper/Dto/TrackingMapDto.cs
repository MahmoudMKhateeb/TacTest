using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using TACHYON.Dashboards.Shipper.Dto;
using TACHYON.Routs.RoutPoints;
using TACHYON.Routs.RoutPoints.Dtos;
using TACHYON.Shipping.ShippingRequestStatuses;
using TACHYON.Shipping.Trips;

namespace TACHYON.Dashboards.Shipper.Dto
{
    public class TrackingMapDto : EntityDto<long>
    {
        public string OriginCity { get; set; }
        public string DestinationCity { get; set; }
        public double OriginLongitude { get; set; }
        public double OriginLatitude { get; set; }
        public double DestinationLongitude { get; set; }
        public double DestinationLatitude { get; set; }
        public ShippingRequestTripStatus TripStatus { get; set; }
        public string StatusTitle { get { return TripStatus.GetEnumDescription(); } }
        public string WayBillNumber { get; set; }

        public string ExpectedDeliveryTime { get; set; }
        public string DriverName { get; set; }
        public string TruckType { get; set; }
        public bool HasIncident { get; set; }
        public List<RoutePointsTripDto> RoutPoints { get; set; }
    }
}