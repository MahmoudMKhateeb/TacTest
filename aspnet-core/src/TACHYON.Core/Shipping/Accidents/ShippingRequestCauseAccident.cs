using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TACHYON.Shipping.Accidents
{
    [Table("ShippingRequestCausesAccidents")]
    public class ShippingRequestCauseAccident: FullAuditedEntity
    {
        [Required]
        [StringLength(60,MinimumLength =3)]
        public string DisplayName { get; set; }

    }
}
