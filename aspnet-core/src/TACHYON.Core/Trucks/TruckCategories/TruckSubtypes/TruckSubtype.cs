using TACHYON.Trucks.TrucksTypes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace TACHYON.Trucks.TruckCategories.TruckSubtypes
{
	[Table("TruckSubtypes")]
    public class TruckSubtype : FullAuditedEntity 
    {

		[Required]
		[StringLength(TruckSubtypeConsts.MaxDisplayNameLength, MinimumLength = TruckSubtypeConsts.MinDisplayNameLength)]
		public virtual string DisplayName { get; set; }
		

		public virtual long? TrucksTypeId { get; set; }
		
        [ForeignKey("TrucksTypeId")]
		public TrucksType TrucksTypeFk { get; set; }
		
    }
}