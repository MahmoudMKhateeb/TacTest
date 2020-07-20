using Abp.Application.Services.Dto;

namespace TACHYON.Routs.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}