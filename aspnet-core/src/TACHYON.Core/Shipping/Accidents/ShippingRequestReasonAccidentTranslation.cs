using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TACHYON.Shipping.Accidents
{
    [Table("ShippingRequestReasonAccidentTranslations")]
    public class ShippingRequestReasonAccidentTranslation : Entity, IEntityTranslation<ShippingRequestReasonAccident>
    {
        [Required]
        [StringLength(60, MinimumLength = 3)]
        public string Name { get; set; }
        public ShippingRequestReasonAccident Core { get; set; }
        public int CoreId { get; set; }
        public string Language { get; set; }

    }
}
