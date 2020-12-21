using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace TACHYON.TermsAndConditions
{
	[Table("TermAndConditions")]
    public class TermAndCondition : CreationAuditedEntity , IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		[Required]
		[StringLength(TermAndConditionConsts.MaxTitleLength, MinimumLength = TermAndConditionConsts.MinTitleLength)]
		public virtual string Title { get; set; }
		
		[Required]
		public virtual string Content { get; set; }
		
		public virtual double Version { get; set; }
		
		public virtual int? EditionId { get; set; }

		public virtual bool IsActive { get; set; }


    }
}