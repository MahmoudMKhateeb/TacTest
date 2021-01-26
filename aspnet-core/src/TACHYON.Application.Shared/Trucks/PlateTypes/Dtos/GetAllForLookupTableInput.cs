using Abp.Application.Services.Dto;

namespace TACHYON.Trucks.PlateTypes.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}