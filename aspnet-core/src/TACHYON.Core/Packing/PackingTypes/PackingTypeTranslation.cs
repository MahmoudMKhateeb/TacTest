using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TACHYON.Packing.PackingTypes
{
    [Table("PackingTypesTranslations")]
    public class PackingTypeTranslation : FullAuditedEntity, IEntityTranslation<PackingType>
    {
        [Required]
        [StringLength(PackingTypeConsts.MaxDisplayNameLength, MinimumLength = PackingTypeConsts.MinDisplayNameLength)]
        public virtual string DisplayName { get; set; }

        [StringLength(PackingTypeConsts.MaxDescriptionLength, MinimumLength = PackingTypeConsts.MinDescriptionLength)]
        public virtual string Description { get; set; }

        [Required]
        public string Language { get; set; }
        public PackingType Core { get; set; }
        public int CoreId { get; set; }
    }
}
