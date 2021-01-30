using TACHYON.Trucks.TruckCategories.TruckCapacities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace TACHYON.Trucks.TruckCategories.TruckCapacities.TruckCapacitiesTranslations
{
    [Table("TruckCapacitiesTranslations")]
    public class TruckCapacitiesTranslation : FullAuditedEntity, IEntityTranslation<Capacity>
    {

        [Required]
        [StringLength(TruckCapacitiesTranslationConsts.MaxTranslatedDisplayNameLength, MinimumLength = TruckCapacitiesTranslationConsts.MinTranslatedDisplayNameLength)]
        public virtual string TranslatedDisplayName { get; set; }

        [Required]
        [StringLength(TruckCapacitiesTranslationConsts.MaxLanguageLength, MinimumLength = TruckCapacitiesTranslationConsts.MinLanguageLength)]
        public virtual string Language { get; set; }

        public virtual int CoreId { get; set; }

        [ForeignKey("CoreId")]
        public Capacity Core { get; set; }

    }
}