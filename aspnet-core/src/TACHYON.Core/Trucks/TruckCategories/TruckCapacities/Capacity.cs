using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using TACHYON.Trucks.TrucksTypes;

namespace TACHYON.Trucks.TruckCategories.TruckCapacities
{
	[Table("Capacities")]
    public class Capacity : FullAuditedEntity 
    {

		[Required]
		[StringLength(CapacityConsts.MaxDisplayNameLength, MinimumLength = CapacityConsts.MinDisplayNameLength)]
		public virtual string DisplayName { get; set; }
		

		public virtual long TrucksTypeId { get; set; }
		
        [ForeignKey("TrucksTypeId")]
		public TrucksType TrucksTypeFk { get; set; }
		
    }
}