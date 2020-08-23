using Abp.Auditing;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TACHYON.Trucks
{
    [Table("TruckStatuses")]
    [Audited]
    public class TruckStatus : FullAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }


        [Required]
        [StringLength(TruckStatusConsts.MaxDisplayNameLength, MinimumLength = TruckStatusConsts.MinDisplayNameLength)]
        public virtual string DisplayName { get; set; }


    }
}