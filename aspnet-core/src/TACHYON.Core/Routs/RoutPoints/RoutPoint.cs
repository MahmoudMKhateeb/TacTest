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

namespace TACHYON.Routs.RoutPoints
{
    [Table("RoutPoints")]
    public class RoutPoint: FullAuditedEntity<long>
    {
        public string DisplayName { get; set; }
        //public int TenantId { get; set; }

        //[ForeignKey("TenantId")]
        //public Tenant TenantFk { get; set; }

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
        public virtual long ShippingRequestId { get; set; }

        [ForeignKey("ShippingRequestId")]
        public ShippingRequest ShippingRequestFk { get; set; }

        public ICollection<GoodsDetail> GoodsDetails { get; set; }


        //to do receiver attribute

    }
}
