using Abp.Application.Services.Dto;

namespace TACHYON.UnitOfMeasures.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}