using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

namespace TACHYON.Routs.RoutTypes
{
	[Table("RoutTypes")]
    [Audited]
    public class RoutType : FullAuditedEntity 
    {

		[Required]
		[StringLength(RoutTypeConsts.MaxDisplayNameLength, MinimumLength = RoutTypeConsts.MinDisplayNameLength)]
		public virtual string DisplayName { get; set; }
		
		[StringLength(RoutTypeConsts.MaxDescriptionLength, MinimumLength = RoutTypeConsts.MinDescriptionLength)]
		public virtual string Description { get; set; }
		

    }
}