using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

namespace TACHYON.Trucks.TrucksTypes
{
	[Table("TrucksTypes")]
    [Audited]
    public class TrucksType : FullAuditedEntity<Guid> , IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		[Required]
		[StringLength(TrucksTypeConsts.MaxDisplayNameLength, MinimumLength = TrucksTypeConsts.MinDisplayNameLength)]
		public virtual string DisplayName { get; set; }
		

    }
}