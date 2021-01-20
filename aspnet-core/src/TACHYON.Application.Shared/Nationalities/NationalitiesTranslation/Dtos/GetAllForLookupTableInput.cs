using Abp.Application.Services.Dto;

namespace TACHYON.Nationalities.NationalitiesTranslation.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}