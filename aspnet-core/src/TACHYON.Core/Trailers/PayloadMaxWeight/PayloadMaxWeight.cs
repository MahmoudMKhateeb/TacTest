using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

namespace TACHYON.Trailers.PayloadMaxWeight
{
	[Table("PayloadMaxWeights")]
    [Audited]
    public class PayloadMaxWeight : FullAuditedEntity 
    {

		[Required]
		[StringLength(PayloadMaxWeightConsts.MaxDisplayNameLength, MinimumLength = PayloadMaxWeightConsts.MinDisplayNameLength)]
		public virtual string DisplayName { get; set; }
		
		public virtual int MaxWeight { get; set; }
		

    }
}