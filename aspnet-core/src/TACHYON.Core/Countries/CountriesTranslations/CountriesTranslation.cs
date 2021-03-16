using TACHYON.Countries;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace TACHYON.Countries.CountriesTranslations
{
    [Table("CountriesTranslations")]
    public class CountriesTranslation : FullAuditedEntity, IEntityTranslation<County>
    {
        [Required]
        [StringLength(CountriesTranslationConsts.MaxTranslatedDisplayNameLength, MinimumLength = CountriesTranslationConsts.MinTranslatedDisplayNameLength)]
        public virtual string TranslatedDisplayName { get; set; }

        [Required]
        [StringLength(CountriesTranslationConsts.MaxLanguageLength, MinimumLength = CountriesTranslationConsts.MinLanguageLength)]
        public virtual string Language { get; set; }

        public virtual int CoreId { get; set; }

        [ForeignKey("CoreId")]
        public County Core { get; set; }

    }
}