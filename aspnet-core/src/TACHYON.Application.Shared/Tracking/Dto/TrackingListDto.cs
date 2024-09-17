using Abp.Application.Services.Dto;
using System;
using TACHYON.Authorization.Users.Profile.Dto;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.Trips;

namespace TACHYON.Tracking.Dto
{
    public class TrackingListDto : EntityDto
    {
        public long? CreatorUserId { get; set; }
        public string Name { get; set; }
        public string ShipperName { get; set; }
        public string CarrierName { get; set; }
        public ShippingRequestTripStatus Status { get; set; }
        public string StatusTitle { get { return Status.GetEnumDescription(); } set { } }
        public string Driver { get; set; }
        public ShippingRequestTripCancelStatus CancelStatus { get; set; }
        public decimal DriverRate { get; set; }
        public ShippingRequestTripDriverStatus DriverStatus { get; set; }
        public string DriverStatusTitle { get { return DriverStatus.GetEnumDescription(); } set { } }
        public long? AssignedDriverUserId { get; set; }
        public string TenantPhoto { get; set; }
        public Guid? DriverImageProfile { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public bool NeedsDeliveryNote { get; set; }
        public int NumberOfDrops { get; set; }
        public long? RequestId { get; set; }
        public string TruckType { get; set; }
        public string GoodsCategory { get; set; }
        public ShippingRequestRouteType RouteTypeId { get; set; }
        public string RouteType { get { return RouteTypeId.GetEnumDescription(); } set { } }
        public string Reason { get; set; }
        public bool IsAssign { get; set; }
        public bool CanStartTrip { get; set; }
        public ShippingRequestType RequestType { get; set; }
        public bool HasAccident { get; set; }
        public string NoActionReason { get; set; }
        public bool CanAcceptTrip { get; set; }
        public bool isApproveCancledByCarrier { get; set; }
        public bool isApproveCancledByShipper { get; set; }
        public bool IsApproveCancledByTachyonDealer { get; set; }
        public bool IsForcedCanceledByTachyonDealer { get; set; }
        public string CanceledReason { get; set; }

        public long? WaybillNumber { get; set; }
        public bool IsSass { get; set; }
        public string ReferenceNumber { get; set; }

        /// <summary>
        /// shipper Id
        /// </summary>
        public int TenantId { get; set; }
        public ShippingRequestStatus shippingRequestStatus { get; set; }
        public bool? IsPrePayedShippingRequest { get; set; }
        public bool CanDriveTrip { get; set; }
        public ShippingRequestFlag ShippingRequestFlag { get; set; }
        public string ShippingRequestFlagTitle { get; set; }

        public bool IsTripImpactEnabled { get; set; }

        public ShippingTypeEnum ShippingType { get; set; }
        public string ShippingTypeTitle { get; set; }
        public string ContainerNumber { get; set; }
        public string PlateNumber { get; set; }
        public string BookingNumber { get; set; }

        #region Dedicated
        public int NumberOfTrucks { get; set; }

        public ShippingRequestTripFlag TripFlag { get; set; }
        #endregion

        public string BayanId { get; set; }


        public long? ShipperInvoiceNumber { get; set; }
        public long? CarrierInvoiceNumber { get; set; }
        public int? CarrierTenantId { get; set; }
        public DateTime StartTripDate { get; set; }
        public string ActualDeliveryDate { get; set; }
        public string ActualPickupDate { get; set; }
        public bool IsPODUploaded { get; set; }
        public bool IsInvoiceIssued { get; set; }
        public string SabOrderId { get; set; }

        public DateTime? ContainerReturnDate { get; set; }
        public bool? IsContainerReturned {get;set;}

        public string ShipperReference { get; set; }

        public decimal? ActorShipperTotalAmountWithCommission { get; set; }
        public decimal? ActorShipperSubTotalAmountWithCommission { get; set; }

        public long? ReplacesDriverId {get;set;}

        public string ReplacedDriver { get; set; }

        
        
    }
}