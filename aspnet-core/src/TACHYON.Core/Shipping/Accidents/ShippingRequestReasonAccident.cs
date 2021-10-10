using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.Common;

namespace TACHYON.Shipping.Accidents
{
    [Table("ShippingRequestReasonAccidents")]
    public class ShippingRequestReasonAccident : FullAuditedEntity, IMultiLingualEntity<ShippingRequestReasonAccidentTranslation>, IHasKey
    {
        [Required]
        [StringLength(60, MinimumLength = 3)]
        public string Key { get; set; }
        public ICollection<ShippingRequestReasonAccidentTranslation> Translations { get; set; }
    }
}