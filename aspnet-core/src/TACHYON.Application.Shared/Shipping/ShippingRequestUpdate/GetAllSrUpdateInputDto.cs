using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Shipping.ShippingRequestUpdate
{
    public class GetAllSrUpdateInputDto : PagedAndSortedResultRequestDto
    {
        [Required]
        public long PriceOfferId { get; set; }

        [Required]
        public long ShippingRequestId { get; set; }
    }
}