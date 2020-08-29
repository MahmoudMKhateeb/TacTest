using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace TACHYON.UnitOfMeasures
{
	[Table("UnitOfMeasures")]
    public class UnitOfMeasure : FullAuditedEntity 
    {

		[Required]
		[StringLength(UnitOfMeasureConsts.MaxDisplayNameLength, MinimumLength = UnitOfMeasureConsts.MinDisplayNameLength)]
		public virtual string DisplayName { get; set; }
		

    }
}