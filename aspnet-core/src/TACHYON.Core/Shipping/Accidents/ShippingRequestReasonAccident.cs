using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TACHYON.Shipping.Accidents
{
    [Table("ShippingRequestReasonAccidents")]
    public class ShippingRequestReasonAccident: FullAuditedEntity, IMultiLingualEntity<ShippingRequestReasonAccidentTranslation>
    {
        public ICollection<ShippingRequestReasonAccidentTranslation> Translations { get; set; }
    }
}
