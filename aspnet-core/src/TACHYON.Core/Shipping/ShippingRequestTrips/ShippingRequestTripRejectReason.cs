using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TACHYON.Shipping.ShippingRequestTrips
{
    [Table("ShippingRequestTripRejectReasons")]
    public class ShippingRequestTripRejectReason : FullAuditedEntity, IMultiLingualEntity<ShippingRequestTripRejectReasonTranslation>
    {
        [Required]
        [StringLength(500, MinimumLength = 3)]
        public string DisplayName { get; set; }
        public ICollection<ShippingRequestTripRejectReasonTranslation> Translations { get; set; }

    }
}
