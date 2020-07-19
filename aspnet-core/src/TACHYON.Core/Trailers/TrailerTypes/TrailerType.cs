using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

namespace TACHYON.Trailers.TrailerTypes
{
	[Table("TrailerTypes")]
    [Audited]
    public class TrailerType : FullAuditedEntity 
    {

		[Required]
		[StringLength(TrailerTypeConsts.MaxDisplayNameLength, MinimumLength = TrailerTypeConsts.MinDisplayNameLength)]
		public virtual string DisplayName { get; set; }
		

    }
}