using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Common;
using TACHYON.Trucks.TruckCategories.TransportTypes;

namespace TACHYON.Trucks.TruckCategories.TransportTypes.TransportTypesTranslations
{
    [Table("TransportTypesTranslations")]
    public class TransportTypesTranslation : FullAuditedEntity, IEntityTranslation<TransportType>, IHasDisplayName

    {
        [StringLength(TransportTypesTranslationConsts.MaxDisplayNameLength,
            MinimumLength = TransportTypesTranslationConsts.MinDisplayNameLength)]
        public virtual string TranslatedDisplayName { get; set; }


        [Required]
        [StringLength(TransportTypesTranslationConsts.MaxLanguageLength,
            MinimumLength = TransportTypesTranslationConsts.MinLanguageLength)]
        public virtual string Language { get; set; }

        public virtual int CoreId { get; set; }

        [ForeignKey("CoreId")] public TransportType Core { get; set; }

        [Required]
        [StringLength(TransportTypesTranslationConsts.MaxDisplayNameLength,
            MinimumLength = TransportTypesTranslationConsts.MinDisplayNameLength)]
        public string DisplayName { get; set; }
    }
}