using Abp.Application.Services.Dto;

namespace TACHYON.ShippingRequestVases.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}