using Abp.Auditing;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.AddressBook;
using TACHYON.Authorization.Users;
using TACHYON.Integration.WaslIntegration;
using TACHYON.Penalties;
using TACHYON.PriceOffers;
using TACHYON.Rating;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.Trips;
using TACHYON.ShippingRequestTripVases;
using TACHYON.Trucks;

namespace TACHYON.Shipping.ShippingRequestTrips
{
    [Audited]
    [Table("ShippingRequestTrips")]
    public class ShippingRequestTrip : FullAuditedEntity, IWaslIntegrated
    {
        public long? WaybillNumber { get; set; }
        public DateTime StartTripDate { get; set; }
        public DateTime? EndTripDate { get; set; }
        public DateTime? StartWorking { get; set; }
        public DateTime? EndWorking { get; set; }
        public bool HasAttachment { get; set; }
        public bool NeedsDeliveryNote { get; set; }
        public ShippingRequestTripStatus Status { get; set; }
        public RoutePointStatus RoutePointStatus { get; set; }

        public long? AssignedDriverUserId { get; set; }

        /// <summary>
        /// Used for worker to reminder the driver to accept trip
        /// </summary>
        public DateTime? AssignedDriverTime { get; set; }

        [ForeignKey("AssignedDriverUserId")] public User AssignedDriverUserFk { get; set; }

        /// <summary>
        /// if the driver make accident when he work on trip
        /// </summary>
        public bool HasAccident { get; set; }

        public bool IsApproveCancledByShipper { get; set; }
        public bool IsApproveCancledByCarrier { get; set; }
        public bool IsApproveCancledByTachyonDealer { get; set; }
        public bool IsForcedCanceledByTachyonDealer { get; set; }
        public long? AssignedTruckId { get; set; }
        [ForeignKey("AssignedTruckId")] public Truck AssignedTruckFk { get; set; }
        public long ShippingRequestId { get; set; }
        [ForeignKey("ShippingRequestId")] public ShippingRequest ShippingRequestFk { get; set; }


        //Facility
        public virtual long? OriginFacilityId { get; set; }

        [ForeignKey("OriginFacilityId")] public Facility OriginFacilityFk { get; set; }

        public virtual long? DestinationFacilityId { get; set; }

        [ForeignKey("DestinationFacilityId")] public Facility DestinationFacilityFk { get; set; }

        public ICollection<RoutPoint> RoutPoints { get; set; }
        public ICollection<ShippingRequestTripVas> ShippingRequestTripVases { get; set; }
        public ICollection<RatingLog> RatingLogs { get; set; }
        public ICollection<Penalty> Penalties { get; set; }
        public ShippingRequestTripDriverStatus DriverStatus { get; set; }
        public int? RejectReasonId { get; set; }

        [ForeignKey("RejectReasonId")]
        public ShippingRequestTripRejectReason ShippingRequestTripRejectReason { get; set; }

        public string RejectedReason { get; set; }

        /// <summary>
        /// approximate total value of goods
        /// </summary>
        public string TotalValue { get; set; }

        /// <summary>
        /// This is a Trip Note Added By Shipper
        /// </summary>
        // Entity Validation Not Required But Best Practice
        [StringLength(ShippingRequestTripConsts.MaxNoteLength)]
        public string Note { get; set; }

        public DateTime? ActualPickupDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }

        #region Prices

        public bool IsShipperHaveInvoice { get; set; }
        public bool IsCarrierHaveInvoice { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? SubTotalAmount { get; set; }
        public decimal? VatAmount { get; set; }
        public decimal? TotalAmountWithCommission { get; set; }
        public decimal? SubTotalAmountWithCommission { get; set; }
        public decimal? VatAmountWithCommission { get; set; }
        public decimal? TaxVat { get; set; }
        public PriceOfferCommissionType? CommissionType { get; set; }
        public decimal? CommissionPercentageOrAddValue { get; set; }
        public decimal? CommissionAmount { get; set; }


        #endregion
        //integrations
        public string BayanId { get; set; }
        public bool IsWaslIntegrated { get; set; }
        public string WaslIntegrationErrorMsg { get; set; }

    }
}