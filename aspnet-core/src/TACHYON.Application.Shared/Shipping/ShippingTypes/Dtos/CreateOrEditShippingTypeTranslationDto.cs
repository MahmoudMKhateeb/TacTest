using System.ComponentModel.DataAnnotations;

namespace TACHYON.Shipping.ShippingTypes.Dtos
{
    public class CreateOrEditShippingTypeTranslationDto : CreateOrEditShippingTypeDto
    {
        [Required]
        [StringLength(5,MinimumLength = 2)]
        public string Language { get; set; }

        [Required]
        public int CoreId { get; set; }

    }
}
