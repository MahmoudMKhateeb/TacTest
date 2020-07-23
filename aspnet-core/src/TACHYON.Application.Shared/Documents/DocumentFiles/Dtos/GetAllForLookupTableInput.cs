using Abp.Application.Services.Dto;

namespace TACHYON.Documents.DocumentFiles.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}