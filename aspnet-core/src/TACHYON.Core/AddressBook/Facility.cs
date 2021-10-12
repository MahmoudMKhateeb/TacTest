using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Cities;
using TACHYON.Countries;

namespace TACHYON.AddressBook
{
    [Table("Facilities")]
    public class Facility : AddressBaseFullAuditedEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public override string Name { get; set; }
        /// <summary>
        /// final rate of the facility
        /// </summary>
        public decimal Rate { get; set; }
        public int RateNumber { get; set; }
    }
}