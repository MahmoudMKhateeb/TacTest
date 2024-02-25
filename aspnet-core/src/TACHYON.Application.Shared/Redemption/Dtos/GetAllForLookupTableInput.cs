using Abp.Application.Services.Dto;

namespace TACHYON.Redemption.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}