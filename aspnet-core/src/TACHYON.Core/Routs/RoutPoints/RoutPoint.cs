using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.AddressBook;
using TACHYON.Cities;
using TACHYON.MultiTenancy;
using TACHYON.PickingTypes;
using TACHYON.Routs.RoutSteps;

namespace TACHYON.Routs.RoutPoints
{
    [Table("RoutPoints")]
    public class RoutPoint: FullAuditedEntity<long>, IMustHaveTenant
    {
        public string DisplayName { get; set; }
        public int TenantId { get; set; }

        [ForeignKey("TenantId")]
        public Tenant TenantFk { get; set; }
        [Required]
        public int CityId { get; set; }

        [ForeignKey("CityId")]
        public City CityFk { get; set; }

        /// <summary>
        /// pickup or droppoff or null
        /// </summary>
        public int? PickingTypeId { get; set; }

        [ForeignKey("PickingTypeId")]
        public PickingType PickingTypeFk { get; set; }

        [Required]
        [StringLength(RoutStepConsts.MaxLatitudeLength, MinimumLength = RoutStepConsts.MinLatitudeLength)]
        public virtual string Latitude { get; set; }

        [Required]
        [StringLength(RoutStepConsts.MaxLatitudeLength, MinimumLength = RoutStepConsts.MinLatitudeLength)]
        public virtual string Longitude { get; set; }

        /// <summary>
        /// address book for this point
        /// </summary>
        [Required]
        public long FacilityId { get; set; }

        [ForeignKey("SourceFacilityId")]
        public Facility FacilityFk { get; set; }

        public ICollection<RoutPointGoodsDetail> RoutPointGoodsDetails { get; set; }

        //to do receiver attribute

    }
}
