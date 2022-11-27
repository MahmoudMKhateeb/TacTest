using Abp.Application.Services.Dto;

namespace TACHYON.Regions.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}