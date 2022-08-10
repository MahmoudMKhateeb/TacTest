using System.ComponentModel.DataAnnotations;
using TACHYON.Dto;

namespace TACHYON.Shipping.SrPostPriceUpdates
{
    public class GetSrPostPriceUpdatesInput : PagedAndSortedInputDto
    {
        [Required]
        public long ShippingRequestId { get; set; }
    }
}