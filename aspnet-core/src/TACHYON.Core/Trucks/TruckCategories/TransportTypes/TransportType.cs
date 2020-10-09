using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace TACHYON.Trucks.TruckCategories.TransportTypes
{
	[Table("TransportTypes")]
    public class TransportType : FullAuditedEntity 
    {

		[Required]
		[StringLength(TransportTypeConsts.MaxDisplayNameLength, MinimumLength = TransportTypeConsts.MinDisplayNameLength)]
		public virtual string DisplayName { get; set; }
		

    }
}