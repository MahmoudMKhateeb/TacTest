using Abp.Application.Services.Dto;

namespace TACHYON.Nationalities.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}