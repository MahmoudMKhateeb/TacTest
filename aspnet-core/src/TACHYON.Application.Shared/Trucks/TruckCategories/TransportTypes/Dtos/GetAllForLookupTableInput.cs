using Abp.Application.Services.Dto;

namespace TACHYON.Trucks.TruckCategories.TransportTypes.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}