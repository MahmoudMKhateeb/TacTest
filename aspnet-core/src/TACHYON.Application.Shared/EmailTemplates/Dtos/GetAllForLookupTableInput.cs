using Abp.Application.Services.Dto;

namespace TACHYON.EmailTemplates.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}