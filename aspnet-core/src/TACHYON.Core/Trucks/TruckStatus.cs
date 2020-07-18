using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

namespace TACHYON.Trucks
{
	[Table("TruckStatuses")]
    [Audited]
    public class TruckStatus : FullAuditedEntity<Guid> , IMustHaveTenant
    {
			public int TenantId { get; set; }
			

		[Required]
		[StringLength(TruckStatusConsts.MaxDisplayNameLength, MinimumLength = TruckStatusConsts.MinDisplayNameLength)]
		public virtual string DisplayName { get; set; }
		

    }
}