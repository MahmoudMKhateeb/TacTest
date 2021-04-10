using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TACHYON.Shipping.ShippingRequestTrips
{
    [Table("ShippingRequestTripRejectReasons")]
    public class ShippingRequestTripRejectReason : FullAuditedEntity
    {
        [Required]
        [StringLength(500, MinimumLength = 3)]
        public string DisplayName { get; set; }
    }
}
