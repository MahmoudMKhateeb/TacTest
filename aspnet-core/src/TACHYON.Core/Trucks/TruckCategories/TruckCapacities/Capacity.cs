using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using System.Collections.Generic;
using TACHYON.Trucks.TruckCategories.TruckCapacities.TruckCapacitiesTranslations;
using TACHYON.Trucks.TrucksTypes;

namespace TACHYON.Trucks.TruckCategories.TruckCapacities
{
	[Table("Capacities")]
    public class Capacity : FullAuditedEntity, IMultiLingualEntity<TruckCapacitiesTranslation>
    {

		[Required]
		[StringLength(CapacityConsts.MaxDisplayNameLength, MinimumLength = CapacityConsts.MinDisplayNameLength)]
		public virtual string DisplayName { get; set; }
        
		public virtual long TrucksTypeId { get; set; }
		
        [ForeignKey("TrucksTypeId")]
		public TrucksType TrucksTypeFk { get; set; }

        public ICollection<TruckCapacitiesTranslation> Translations { get; set; }


    }
}