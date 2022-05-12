using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Common;

namespace TACHYON.Shipping.Accidents
{
    [Table("ShippingRequestReasonAccidentTranslations")]
    public class ShippingRequestReasonAccidentTranslation : Entity, IEntityTranslation<ShippingRequestReasonAccident>,
        IHasDisplayName
    {
        public string Name { get; set; }
        public ShippingRequestReasonAccident Core { get; set; }
        public int CoreId { get; set; }
        [Required] public string Language { get; set; }

        [Required]
        [StringLength(60, MinimumLength = 3)]
        public string DisplayName { get; set; }
    }
}