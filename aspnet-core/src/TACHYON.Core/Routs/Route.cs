using TACHYON.Routs.RoutTypes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

namespace TACHYON.Routs
{
	[Table("Routes")]
    [Audited]
    public class Route : FullAuditedEntity , IMustHaveTenant
    {
			public int TenantId { get; set; }
			

		[Required]
		[StringLength(RouteConsts.MaxDisplayNameLength, MinimumLength = RouteConsts.MinDisplayNameLength)]
		public virtual string DisplayName { get; set; }
		
		[StringLength(RouteConsts.MaxDescriptionLength, MinimumLength = RouteConsts.MinDescriptionLength)]
		public virtual string Description { get; set; }
		

		public virtual int? RoutTypeId { get; set; }
		
        [ForeignKey("RoutTypeId")]
		public RoutType RoutTypeFk { get; set; }
		
    }
}