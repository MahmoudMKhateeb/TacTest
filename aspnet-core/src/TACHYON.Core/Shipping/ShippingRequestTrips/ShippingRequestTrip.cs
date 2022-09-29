using Abp;
using Abp.Auditing;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.AddressBook;
using TACHYON.Authorization.Users;
using TACHYON.Integration.WaslIntegration;
using TACHYON.Invoices.ActorInvoices;
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
        public ShippingRequestTripCancelStatus CancelStatus { get; set; }
        public RoutePointStatus RoutePointStatus { get; set; }

        public DateTime? ExpectedDeliveryTime { get; set; }
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
        /// <summary>
        /// this column will be true if shipper make or approve cancel the trip
        /// </summary>
        public bool IsApproveCancledByShipper { get; set; }
        /// <summary>
        /// this column will be true if carrier make or approve cancel the trip
        /// </summary>
        public bool IsApproveCancledByCarrier { get; set; }
        /// <summary>
        /// this column will be true if TMS make or approve cancel the trip
        /// </summary>
        public bool IsApproveCancledByTachyonDealer { get; set; }
        //todo this will be removed .. TMS always force cancel the trip
        public bool IsForcedCanceledByTachyonDealer { get; set; }
        /// <summary>
        /// When Shipper or carrier or TMS cancel trip, canceled reason should be filled.
        /// </summary>
        public string CanceledReason { get; set; }
        /// <summary>
        /// When TMS reject cancelation from tenant, Reject Canceled Reason should be filled
        /// </summary>
        public string RejectedCancelingReason { get; set; }
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
        public InvoiceTripStatus InvoiceStatus { get; set; }
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
        /// <summary>
        /// This field entered by shipper to suppose time for driver to start with in this range
        /// </summary>
        public DateTime? SupposedPickupDateFrom { get; set; }
        public DateTime? SupposedPickupDateTo { get; set; }

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
        /// <summary>
        /// This reference is for import shipment from excel, it is unique reference for user to know the trips whick contains errors,
        /// it is either entered menual or auto generated. it is unique in each request.
        /// </summary>
        public string BulkUploadRef { get; set; } 
            = DateTime.Now.ToString("dd") + DateTime.Now.ToString("HH") + RandomHelper.GetRandom(10, 99);

        #region Remarks
        public bool CanBePrinted { get; set; }
        public string RoundTrip { get; set; }
        public string ContainerNumber { get; set; }
        #endregion

        public ActorCarrierPrice ActorCarrierPriceFk { get; set; }



        public ActorShipperPrice ActorShipperPriceFk { get; set; }


        public long? ActorInvoiceId { get; set; }

        [ForeignKey("ActorInvoiceId")]
        public ActorInvoice ActorInvoiceFk { get; set; }

        public long? ActorSubmitInvoiceId { get; set; }

        [ForeignKey("ActorSubmitInvoiceId")]
        public ActorSubmitInvoice ActorSubmitInvoiceFk { get; set; }
    }
}