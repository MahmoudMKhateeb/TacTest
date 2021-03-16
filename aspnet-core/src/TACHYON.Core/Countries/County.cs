using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Countries.CountriesTranslations;

namespace TACHYON.Countries
{
    [Table("Counties")]
    public class County : FullAuditedEntity, IMultiLingualEntity<CountriesTranslation>
    {

        [Required]
        [StringLength(CountyConsts.MaxDisplayNameLength, MinimumLength = CountyConsts.MinDisplayNameLength)]
        public virtual string DisplayName { get; set; }

        [Required]
        [StringLength(CountyConsts.MaxCodeLength, MinimumLength = CountyConsts.MinCodeLength)]
        public virtual string Code { get; set; }

        public ICollection<CountriesTranslation> Translations { get; set; }

    }
}