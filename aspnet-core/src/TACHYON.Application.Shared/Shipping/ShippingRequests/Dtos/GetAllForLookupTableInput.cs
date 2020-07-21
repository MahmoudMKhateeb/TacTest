using Abp.Application.Services.Dto;

namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}