using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TACHYON.Shipping.ShippingRequestTrips
{
    [Table("ShippingRequestTripRejectReasonTranslations")]
    public class ShippingRequestTripRejectReasonTranslation : Entity,
        IEntityTranslation<ShippingRequestTripRejectReason>
    {
        [Required]
        [StringLength(60, MinimumLength = 3)]
        public string Name { get; set; }

        public ShippingRequestTripRejectReason Core { get; set; }
        public int CoreId { get; set; }
        [Required] public string Language { get; set; }
    }
}