using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace TACHYON.Goods
{
	[Table("DangerousGoodTypes")]
    public class DangerousGoodType : FullAuditedEntity 
    {

		[Required]
		[StringLength(DangerousGoodTypeConsts.MaxNameLength, MinimumLength = DangerousGoodTypeConsts.MinNameLength)]
		public virtual string Name { get; set; }
        /// <summary>
        /// for Bayan integration mapping Dangerous Good Types
        /// </summary>
		public virtual int? BayanIntegrationId { get; set; }
		

    }
}