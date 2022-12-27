using Abp.Domain.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.MultiTenancy;
using TACHYON.Actors;
using TACHYON.Rating;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.AddressBook
{
    [Table("Facilities")]
    public class Facility : AddressBaseFullAuditedEntity, IMayHaveTenant, IHasRating, IMayHaveShipperActor
    {
        public int? TenantId { get; set; }
        [ForeignKey("TenantId")]
        public Tenant Tenant { get; set; }
        public override string Name { get; set; }

        /// <summary>
        /// final rate of the facility
        /// </summary>
        public decimal Rate { get; set; }

        public int RateNumber { get; set; }
        public ICollection<FacilityWorkingHour> FacilityWorkingHours { get; set; }

        [ForeignKey("ShipperActorId")]
        public Actor ShipperActorFk { get; set; }

        public int? ShipperActorId { get; set ; }
    }
}