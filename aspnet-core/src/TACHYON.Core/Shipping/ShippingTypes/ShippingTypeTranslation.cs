using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TACHYON.Shipping.ShippingTypes
{
    [Table("ShippingTypeTranslations")]
    public class ShippingTypeTranslation : FullAuditedEntity, IEntityTranslation<ShippingType>
    {
        [Required]
        [StringLength(ShippingTypeConsts.MaxDisplayNameLength, MinimumLength = ShippingTypeConsts.MinDisplayNameLength)]
        public virtual string DisplayName { get; set; }

        [StringLength(ShippingTypeConsts.MaxDescriptionLength, MinimumLength = ShippingTypeConsts.MinDescriptionLength)]
        public virtual string Description { get; set; }

        public string Language { get; set; }
        public ShippingType Core { get; set; }
        public int CoreId { get; set; }
    }
}