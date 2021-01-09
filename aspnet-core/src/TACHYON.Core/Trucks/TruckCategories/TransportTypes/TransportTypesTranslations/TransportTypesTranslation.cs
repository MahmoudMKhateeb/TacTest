using TACHYON.Trucks.TruckCategories.TransportTypes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace TACHYON.Trucks.TruckCategories.TransportTypes.TransportTypesTranslations
{
    [Table("TransportTypesTranslations")]
    public class TransportTypesTranslation : FullAuditedEntity, IEntityTranslation<TransportType>

    {
        
        [Required]
        [StringLength(TransportTypesTranslationConsts.MaxDisplayNameLength, MinimumLength = TransportTypesTranslationConsts.MinDisplayNameLength)]
        public virtual string TranslatedDisplayName { get; set; }



        [Required]
        [StringLength(TransportTypesTranslationConsts.MaxLanguageLength, MinimumLength = TransportTypesTranslationConsts.MinLanguageLength)]
        public virtual string Language { get; set; }

        public virtual int CoreId { get; set; }

        [ForeignKey("CoreId")]
        public TransportType Core { get; set; }

    }
}