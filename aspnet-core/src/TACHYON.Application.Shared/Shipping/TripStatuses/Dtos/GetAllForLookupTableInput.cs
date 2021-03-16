using Abp.Application.Services.Dto;

namespace TACHYON.Shipping.TripStatuses.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}