using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Common;
using TACHYON.Trucks.TrucksTypes;

namespace TACHYON.Trucks.TrucksTypes.TrucksTypesTranslations
{
    [Table("TrucksTypesTranslations")]
    public class TrucksTypesTranslation : FullAuditedEntity, IEntityTranslation<TrucksType, long>, IHasDisplayName
    {


        [StringLength(TrucksTypesTranslationConsts.MaxTranslatedDisplayNameLength, MinimumLength = TrucksTypesTranslationConsts.MinTranslatedDisplayNameLength)]
        public virtual string TranslatedDisplayName { get; set; }

        [Required]
        [StringLength(TrucksTypesTranslationConsts.MaxLanguageLength, MinimumLength = TrucksTypesTranslationConsts.MinLanguageLength)]
        public virtual string Language { get; set; }

        public virtual long CoreId { get; set; }

        [ForeignKey("CoreId")]
        public TrucksType Core { get; set; }

        [Required]
        [StringLength(TrucksTypesTranslationConsts.MaxTranslatedDisplayNameLength,
            MinimumLength = TrucksTypesTranslationConsts.MinTranslatedDisplayNameLength)]
        public string DisplayName { get; set; }
    }
}