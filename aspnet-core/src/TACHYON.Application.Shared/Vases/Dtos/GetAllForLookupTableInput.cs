using Abp.Application.Services.Dto;

namespace TACHYON.Vases.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}