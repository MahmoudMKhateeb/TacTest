using Abp.Application.Services.Dto;

namespace TACHYON.Trucks.TruckCategories.TransportSubtypes.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}