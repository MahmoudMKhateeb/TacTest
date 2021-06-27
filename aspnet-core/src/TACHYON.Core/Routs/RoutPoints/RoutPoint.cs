using Abp.Domain.Entities.Auditing;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.AddressBook;
using TACHYON.Goods.GoodsDetails;
using TACHYON.Receivers;
using TACHYON.Shipping.ShippingRequestTrips;

namespace TACHYON.Routs.RoutPoints
{
    [Table("RoutPoints")]
    public class RoutPoint: FullAuditedEntity<long>
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

        [ForeignKey("FacilityId")]
        public Facility FacilityFk { get; set; }

        [Required]
        public virtual int ShippingRequestTripId { get; set; }

        [ForeignKey("ShippingRequestTripId")]
        public ShippingRequestTrip ShippingRequestTripFk { get; set; }

        public ICollection<GoodsDetail> GoodsDetails { get; set; }

        public string Code { get; set; }
        //public RoutePointStatus Status { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool IsActive { get; set; } 
        public bool IsComplete { get; set; }

        public Guid? DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentContentType { get; set; }

        public double? Rating { get; set; }
        [CanBeNull] public string ReceiverNote { get; set; }
        //to do receiver attribute

        //If Receiver as user  in tachyon
        public int? ReceiverId { get; set; }
        [ForeignKey("ReceiverId")]
        public Receiver ReceiverFk { get; set; }
        //Receiver Info if he is outside the platform
        [CanBeNull] public string ReceiverFullName { get; set; }
        [DataType(DataType.PhoneNumber)] [CanBeNull] public string ReceiverPhoneNumber { get; set; }
        [CanBeNull] public string ReceiverEmailAddress { get; set; }
        [CanBeNull] public string ReceiverCardIdNumber { get; set; }

        //Shipper Note
        [CanBeNull] public string Note { get; set; }


    }
}
