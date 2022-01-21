using System.ComponentModel.DataAnnotations;

namespace TACHYON.Shipping.Accidents.Dto
{
    public class ShippingRequestReasonAccidentTranslationDto
    {
        [StringLength(60, MinimumLength = 3)]
        [Required]
        public string Name { get; set; }

        [Required] public string Language { get; set; }

        public string Icon { get; set; }

        public string DisplayName { get; set; }
    }
}