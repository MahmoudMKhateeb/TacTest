using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using System.Collections.Generic;

namespace TACHYON.Shipping.ShippingTypes
{
    [Table("ShippingTypes")]
    public class ShippingType : FullAuditedEntity, IMultiLingualEntity<ShippingTypeTranslation>
    {

        [Required]
        [StringLength(ShippingTypeConsts.MaxDisplayNameLength, MinimumLength = ShippingTypeConsts.MinDisplayNameLength)]
        public virtual string DisplayName { get; set; }

        public ICollection<ShippingTypeTranslation> Translations { get; set; }
    }
}