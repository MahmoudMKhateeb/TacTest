using Abp.Application.Services.Dto;
using System;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.Trips;

namespace TACHYON.Tracking.Dto
{
    public class TrackingSearchInputDto : PagedAndSortedResultRequestDto
    {
        public ShippingRequestTripStatus? Status { get; set; }
        public string Shipper { get; set; }
        public string Carrier { get; set; }
        public long? WaybillNumber { get; set; }
        public int? TransportTypeId { get; set; }
        public int? TruckTypeId { get; set; }
        public int? TruckCapacityId { get; set; }

        public int? OriginId { get; set; }

        public int? DestinationId { get; set; }

        public DateTime? PickupFromDate { get; set; }

        public DateTime? PickupToDate { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }
        public string ReferenceNumber { get; set; }
        public ShippingRequestRouteType? RouteTypeId { get; set; }
        public int? PackingTypeId { get; set; }
        public int? GoodsOrSubGoodsCategoryId { get; set; }
        public string PlateNumberId { get; set; }
        public string DriverNameOrMobile { get; set; }

        public DateTime? DeliveryFromDate { get; set; }

        public DateTime? DeliveryToDate { get; set; }
        public string ContainerNumber { get; set; }
        public bool? IsInvoiceIssued { get; set; }
        public bool? IsSubmittedPOD { get; set; }
        /// <summary>
        /// 1-- truck aggregation "Normal"
        /// 2-- SAAS
        /// 3-- Dedicated
        /// </summary>
        public int? RequestTypeId { get; set; } 

    }
}