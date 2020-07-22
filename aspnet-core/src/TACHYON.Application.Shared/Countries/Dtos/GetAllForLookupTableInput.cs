using Abp.Application.Services.Dto;

namespace TACHYON.Countries.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}