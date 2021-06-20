using Abp.Application.Services.Dto;
using System;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.Trips;

namespace TACHYON.Tracking.Dto
{
    public  class TrackingSearchInputDto: PagedAndSortedResultRequestDto
    {
        public ShippingRequestTripStatus? Status { get; set; }
        public string Shipper { get; set; }
        public string Carrier { get; set; }
        public int? TruckTypeId { get; set; }

        public int? OriginId { get; set; }

        public int? DestinationId { get; set; }

        public DateTime? PickupFromDate { get; set; }

        public DateTime? PickupToDate { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public ShippingRequestRouteType? RouteTypeId { get; set; }

    }
}
