using Abp.Application.Services.Dto;

namespace TACHYON.Documents.DocumentTypes.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}