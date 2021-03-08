using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.AddressBook;
using TACHYON.Goods.GoodsDetails;
using TACHYON.MultiTenancy;
using TACHYON.PickingTypes;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;

namespace TACHYON.Routs.RoutPoints
{
    [Table("RoutPoints")]
    public class RoutPoint: FullAuditedEntity<long>
    {
        public string DisplayName { get; set; }
        public long? ParentId { get; set; }
        [ForeignKey("ParentId")]
        public RoutPoint RoutPointFk { get; set; }

        /// <summary>
        /// pickup or droppoff or null
        /// </summary>
        public int? PickingTypeId { get; set; }

        [ForeignKey("PickingTypeId")]
        public PickingType PickingTypeFk { get; set; }
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

        public string Code { get; set; } = (new Random().Next(100000, 999999)).ToString();

        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public bool IsActive { get; set; }
        public bool IsComplete { get; set; }

        public Guid? DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentContentType { get; set; }

        public int? Rating { get; set; }

        //to do receiver attribute

    }
}
