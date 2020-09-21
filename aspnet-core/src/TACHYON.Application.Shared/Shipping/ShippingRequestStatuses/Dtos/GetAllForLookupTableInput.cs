using Abp.Application.Services.Dto;

namespace TACHYON.Shipping.ShippingRequestStatuses.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}