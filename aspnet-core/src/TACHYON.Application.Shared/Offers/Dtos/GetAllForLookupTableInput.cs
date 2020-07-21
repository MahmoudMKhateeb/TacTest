using Abp.Application.Services.Dto;

namespace TACHYON.Offers.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}