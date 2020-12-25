using Abp.Application.Services.Dto;

namespace TACHYON.TermsAndConditions.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}