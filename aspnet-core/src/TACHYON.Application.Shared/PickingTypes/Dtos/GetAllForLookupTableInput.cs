using Abp.Application.Services.Dto;

namespace TACHYON.PickingTypes.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}