using TACHYON.Trucks;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace TACHYON.Trucks.TruckStatusesTranslations
{
    [Table("TruckStatusesTranslations")]
    public class TruckStatusesTranslation : FullAuditedEntity, IEntityTranslation<TruckStatus, long>
    {

        [Required]
        [StringLength(TruckStatusesTranslationConsts.MaxTranslatedDisplayNameLength, MinimumLength = TruckStatusesTranslationConsts.MinTranslatedDisplayNameLength)]
        public virtual string TranslatedDisplayName { get; set; }

        [Required]
        [StringLength(TruckStatusesTranslationConsts.MaxLanguageLength, MinimumLength = TruckStatusesTranslationConsts.MinLanguageLength)]
        public virtual string Language { get; set; }

        public virtual long CoreId { get; set; }

        [ForeignKey("CoreId")]
        public TruckStatus Core { get; set; }

    }
}