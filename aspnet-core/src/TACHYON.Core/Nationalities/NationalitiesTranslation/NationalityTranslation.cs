using TACHYON.Nationalities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace TACHYON.Nationalities.NationalitiesTranslation
{
    [Table("NationalityTranslations")]
    public class NationalityTranslation : FullAuditedEntity
    {
        [Required] public virtual string TranslatedName { get; set; }

        [Required] public virtual string Language { get; set; }

        public virtual int CoreId { get; set; }

        [ForeignKey("CoreId")] public Nationality Core { get; set; }
    }
}