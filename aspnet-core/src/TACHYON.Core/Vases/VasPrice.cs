using TACHYON.Vases;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace TACHYON.Vases
{
	[Table("VasPrices")]
    public class VasPrice : FullAuditedEntity , IMustHaveTenant
    {
			public int TenantId { get; set; }
			

		public virtual double? Price { get; set; }
		
		public virtual int? MaxAmount { get; set; }
		
		public virtual int? MaxCount { get; set; }
		

		public virtual int VasId { get; set; }
		
        [ForeignKey("VasId")]
		public Vas VasFk { get; set; }
		
    }
}