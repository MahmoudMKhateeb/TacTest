using Abp.Application.Services.Dto;

namespace TACHYON.Trucks.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}