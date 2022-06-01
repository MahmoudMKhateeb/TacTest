using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using TACHYON.Dashboards.Shipper.Dto;
using TACHYON.Routs.RoutPoints;
using TACHYON.Routs.RoutPoints.Dtos;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestStatuses;
using TACHYON.Shipping.Trips;

namespace TACHYON.Dashboards.Shipper.Dto
{
    public class GetTripsForTrackingInput : PagedAndSortedResultRequestDto
    {

        public long? TruckTypeId { get; set; }

        public ShippingRequestRouteType? RouteType { get; set; }

        public string WaybillNumber { get; set; }

        public string DriverName { get; set; }
        
    }

}