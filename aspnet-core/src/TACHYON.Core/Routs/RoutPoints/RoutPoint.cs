using Abp.Domain.Entities.Auditing;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.AddressBook;
using TACHYON.Goods.GoodsDetails;
using TACHYON.Invoices.PaymentMethods;
using TACHYON.Penalties;
using TACHYON.Rating;
using TACHYON.Receivers;
using TACHYON.Shipping.RoutPoints;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Tracking.AdditionalSteps;

namespace TACHYON.Routs.RoutPoints
{
    [Table("RoutPoints")]
    public class RoutPoint : FullAuditedEntity<long>
    {
        public long? WaybillNumber { get; set; }
        public string DisplayName { get; set; }

        /// <summary>
        /// pickup or droppoff or null
        /// </summary>
        public PickingType PickingType { get; set; }

        /// <summary>
        /// address book for this point, Location, city, address
        /// </summary>
        [Required]
        public long FacilityId { get; set; }

        [ForeignKey("FacilityId")] public Facility FacilityFk { get; set; }

        [Required] public virtual int ShippingRequestTripId { get; set; }

        [ForeignKey("ShippingRequestTripId")] public ShippingRequestTrip ShippingRequestTripFk { get; set; }

        public ICollection<GoodsDetail> GoodsDetails { get; set; }
        public ICollection<RoutPointDocument> RoutPointDocuments { get; set; }
        public ICollection<RatingLog> RatingLogs { get; set; }
        public ICollection<RoutPointStatusTransition> RoutPointStatusTransitions { get; set; }
        public ICollection<Penalty> Penalties { get; set; }
        public string Code { get; set; }
        public RoutePointStatus Status { get; set; }
        public RoutePointCompletedStatus CompletedStatus { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool IsResolve { get; set; }
        public bool IsActive { get; set; }
        public bool IsComplete { get; set; }
        public bool CanGoToNextLocation { get; set; }

        public int WorkFlowVersion { get; set; }
        public int? AdditionalStepWorkFlowVersion { get; set; }

        [CanBeNull] public string ReceiverNote { get; set; }
        //to do receiver attribute

        //If Receiver as user  in tachyon
        //todo rename this field to SenderOrReceiverId, If pickup it is sender, if drop it is receiver
        public int? ReceiverId { get; set; }

        [ForeignKey("ReceiverId")] public Receiver ReceiverFk { get; set; }

        //Receiver Info if he is outside the platform
        [CanBeNull] public string ReceiverFullName { get; set; }

        [DataType(DataType.PhoneNumber)]
        [CanBeNull]
        public string ReceiverPhoneNumber { get; set; }

        [CanBeNull] public string ReceiverCardIdNumber { get; set; }

        [CanBeNull] public string ReceiverEmailAddress { get; set; }

        //Shipper Note
        [CanBeNull] public string Note { get; set; }
        public bool IsDeliveryNoteUploaded { get; set; }
        public bool IsPodUploaded { get; set; }
        public bool IsGoodPictureUploaded { get; set; }
        public DateTime? ActualPickupOrDeliveryDate { get; set; }
        /// <summary>
        /// This field is filled when point is imported from excel, it is unique for each trip
        /// </summary>
        public string BulkUploadReference { get; set; }

        
        //integrations
        public string BayanId { get; set; }

        public List<AdditionalStepTransition> AdditionalStepTransitions { get; set; }

        #region Home delivery
        public DropPaymentMethod? DropPaymentMethod { get; set; }
        public bool NeedsReceiverCode { get; set; }
        public bool NeedsPOD { get; set; }
        public bool IsDeliveryConfiremed { get; set; }

        #endregion

        #region port movement
        public int? PointOrder { get; set; }
        public bool NeedsAppointment { get; set; }
        public bool NeedsClearance { get; set; }
        public DateTime? AppointmentDateTime { get; set; }
        public string AppointmentNumber { get; set; }
        public bool HasClearanceVas { get; set; }
        public bool HasAppointmentVas { get; set; }
        #endregion

        public long? TruckId { set; get; }
        public long? DriverUserId { set; get; }

        public int? StorageDays { get; set; }
        public decimal? StoragePricePerDay { get; set; }

        public int DriverWorkingHour { get; set; }
        public int Distance  { get; set; }
        
    }
}