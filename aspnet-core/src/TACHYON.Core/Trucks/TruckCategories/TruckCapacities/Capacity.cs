using TACHYON.Trucks.TruckCategories.TruckSubtypes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace TACHYON.Trucks.TruckCategories.TruckCapacities
{
	[Table("Capacities")]
    public class Capacity : FullAuditedEntity 
    {

		[Required]
		[StringLength(CapacityConsts.MaxDisplayNameLength, MinimumLength = CapacityConsts.MinDisplayNameLength)]
		public virtual string DisplayName { get; set; }
		

		public virtual int TruckSubtypeId { get; set; }
		
        [ForeignKey("TruckSubtypeId")]
		public TruckSubtype TruckSubtypeFk { get; set; }
		
    }
}