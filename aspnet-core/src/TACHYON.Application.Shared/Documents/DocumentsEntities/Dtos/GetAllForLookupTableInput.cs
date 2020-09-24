using Abp.Application.Services.Dto;

namespace TACHYON.Documents.DocumentsEntities.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}