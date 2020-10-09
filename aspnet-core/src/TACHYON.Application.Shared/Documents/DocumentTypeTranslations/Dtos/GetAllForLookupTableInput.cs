using Abp.Application.Services.Dto;

namespace TACHYON.Documents.DocumentTypeTranslations.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}