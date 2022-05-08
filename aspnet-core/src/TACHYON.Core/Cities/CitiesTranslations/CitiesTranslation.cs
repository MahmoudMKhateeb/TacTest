using TACHYON.Cities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace TACHYON.Cities.CitiesTranslations
{
    [Table("CitiesTranslations")]
    public class CitiesTranslation : FullAuditedEntity, IEntityTranslation<City>
    {
        [Required]
        [StringLength(CitiesTranslationConsts.MaxTranslatedDisplayNameLength,
            MinimumLength = CitiesTranslationConsts.MinTranslatedDisplayNameLength)]
        public virtual string TranslatedDisplayName { get; set; }

        [Required]
        [StringLength(CitiesTranslationConsts.MaxLanguageLength,
            MinimumLength = CitiesTranslationConsts.MinLanguageLength)]
        public virtual string Language { get; set; }

        public virtual int CoreId { get; set; }

        [ForeignKey("CoreId")] public City Core { get; set; }
    }
}