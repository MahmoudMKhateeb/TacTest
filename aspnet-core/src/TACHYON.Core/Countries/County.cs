using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace TACHYON.Countries
{
	[Table("Counties")]
    public class County : FullAuditedEntity 
    {

		[Required]
		[StringLength(CountyConsts.MaxDisplayNameLength, MinimumLength = CountyConsts.MinDisplayNameLength)]
		public virtual string DisplayName { get; set; }
		
		[Required]
		[StringLength(CountyConsts.MaxCodeLength, MinimumLength = CountyConsts.MinCodeLength)]
		public virtual string Code { get; set; }
		

    }
}