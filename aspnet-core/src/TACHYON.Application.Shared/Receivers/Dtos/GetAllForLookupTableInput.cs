using Abp.Application.Services.Dto;

namespace TACHYON.Receivers.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}