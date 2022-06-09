using Abp.Domain.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Rating;

namespace TACHYON.AddressBook
{
    [Table("Facilities")]
    public class Facility : AddressBaseFullAuditedEntity, IMayHaveTenant, IHasRating
    {
        public int? TenantId { get; set; }

        public override string Name { get; set; }

        /// <summary>
        /// final rate of the facility
        /// </summary>
        public decimal Rate { get; set; }

        public int RateNumber { get; set; }
        public ICollection<FacilityWorkingHour> FacilityWorkingHours { get; set; }
    }
}