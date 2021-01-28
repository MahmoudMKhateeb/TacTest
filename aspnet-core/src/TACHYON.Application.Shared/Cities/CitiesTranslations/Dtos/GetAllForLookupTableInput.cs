using Abp.Application.Services.Dto;

namespace TACHYON.Cities.CitiesTranslations.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}