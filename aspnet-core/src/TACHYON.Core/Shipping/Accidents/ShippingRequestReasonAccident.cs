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
    public class ShippingRequestReasonAccident: FullAuditedEntity
    {
        [Required]
        [StringLength(60,MinimumLength =3)]
        public string DisplayName { get; set; }

    }
}
