using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using System.Collections;
using TACHYON.Nationalities.NationalitiesTranslation;
using System.Collections.Generic;

namespace TACHYON.Nationalities
{
    [Table("Nationalities")]
    public class Nationality : FullAuditedEntity, IMultiLingualEntity<NationalityTranslation>
    {
        [Required]
        [StringLength(NationalityConsts.MaxNameLength, MinimumLength = NationalityConsts.MinNameLength)]
        public virtual string Name { get; set; }
        public ICollection<NationalityTranslation> Translations { get; set; }

    }
}