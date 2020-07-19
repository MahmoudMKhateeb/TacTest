using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

namespace TACHYON.Trailers.TrailerStatuses
{
	[Table("TrailerStatuses")]
    [Audited]
    public class TrailerStatus : FullAuditedEntity 
    {

		[Required]
		[StringLength(TrailerStatusConsts.MaxDisplayNameLength, MinimumLength = TrailerStatusConsts.MinDisplayNameLength)]
		public virtual string DisplayName { get; set; }
		

    }
}