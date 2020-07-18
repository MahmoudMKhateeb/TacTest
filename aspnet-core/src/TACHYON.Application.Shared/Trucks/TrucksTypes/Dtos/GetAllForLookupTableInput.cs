using Abp.Application.Services.Dto;

namespace TACHYON.Trucks.TrucksTypes.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}