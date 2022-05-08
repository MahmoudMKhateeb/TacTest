using System.ComponentModel.DataAnnotations;

namespace TACHYON.Shipping.Trips.RejectReasons.Dtos
{
    public class ShippingRequestTripRejectReasonTranslationDto
    {
        [Required]
        [StringLength(60, MinimumLength = 3)]
        public string Name { get; set; }

        [Required] public string Language { get; set; }

        public string Icon { get; set; }

        public string DisplayName { get; set; }
    }
}