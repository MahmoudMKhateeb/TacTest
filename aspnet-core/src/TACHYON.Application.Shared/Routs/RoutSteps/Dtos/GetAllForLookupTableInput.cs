using Abp.Application.Services.Dto;

namespace TACHYON.Routs.RoutSteps.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}