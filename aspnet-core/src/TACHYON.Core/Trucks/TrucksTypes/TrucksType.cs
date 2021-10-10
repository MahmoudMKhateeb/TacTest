using Abp.Auditing;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Trucks.TruckCategories.TransportTypes;
using TACHYON.Trucks.TrucksTypes.TrucksTypesTranslations;

namespace TACHYON.Trucks.TrucksTypes
{
    [Table("TrucksTypes")]
    [Audited]
    public class TrucksType : FullAuditedEntity<long>, IMultiLingualEntity<TrucksTypesTranslation>
    {

        [Required]
        [StringLength(TrucksTypeConsts.MaxDisplayNameLength,
            MinimumLength = TrucksTypeConsts.MinDisplayNameLength)]
        public virtual string DisplayName { get; set; }

        public int? TransportTypeId { get; set; }

        [ForeignKey("TransportTypeId")]
        public TransportType TransportTypeFk { get; set; }

        public ICollection<TrucksTypesTranslation> Translations { get; set; }
        public bool IsActive { get; set; } = true;
    }
}