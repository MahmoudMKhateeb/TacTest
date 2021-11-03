using Abp.Application.Services.Dto;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.Trips;

namespace TACHYON.Tracking.Dto
{
    public class TrackingListDto : EntityDto
    {
        public string Name { get; set; }
        public ShippingRequestTripStatus Status { get; set; }
        public string StatusTitle { get { return Status.GetEnumDescription(); } set { } }
        public string Driver { get; set; }
        public ShippingRequestTripDriverStatus DriverStatus { get; set; }
        public string DriverStatusTitle { get { return DriverStatus.GetEnumDescription(); } set { } }
        public long? AssignedDriverUserId { get; set; }
        public string profilePictureUrl { get; set; }
        public string DriverImageProfile { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public bool NeedsDeliveryNote { get; set; }

        public string TruckType { get; set; }
        public string GoodsCategory { get; set; }
        public ShippingRequestRouteType RouteTypeId { get; set; }
        public string RouteType { get { return RouteTypeId.GetEnumDescription(); } set { } }
        public string Reason { get; set; }
        public bool IsAssign { get; set; }
        public bool CanStartTrip { get; set; }
        public ShippingRequestType RequestType { get; set; }
        public bool HasAccident { get; set; }

        public bool isApproveCancledByCarrier { get; set; }
        public bool isApproveCancledByShipper { get; set; }
        public bool IsApproveCancledByTachyonDealer { get; set; }
        public bool IsForcedCanceledByTachyonDealer { get; set; }
        public long? WaybillNumber { get; set; }

        public string ReferenceNumber { get; set; }

        /// <summary>
        /// shipper Id
        /// </summary>
        public int TenantId { get; set; }
    }
}