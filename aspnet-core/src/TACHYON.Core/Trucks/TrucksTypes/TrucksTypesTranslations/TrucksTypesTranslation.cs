using TACHYON.Trucks.TrucksTypes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace TACHYON.Trucks.TrucksTypes.TrucksTypesTranslations
{
    [Table("TrucksTypesTranslations")]
    public class TrucksTypesTranslation : FullAuditedEntity, IEntityTranslation<TrucksType, long>
    {

        [Required]
        [StringLength(TrucksTypesTranslationConsts.MaxTranslatedDisplayNameLength, MinimumLength = TrucksTypesTranslationConsts.MinTranslatedDisplayNameLength)]
        public virtual string TranslatedDisplayName { get; set; }

        [Required]
        [StringLength(TrucksTypesTranslationConsts.MaxLanguageLength, MinimumLength = TrucksTypesTranslationConsts.MinLanguageLength)]
        public virtual string Language { get; set; }

        public virtual long CoreId { get; set; }

        [ForeignKey("CoreId")]
        public TrucksType Core { get; set; }

    }
}