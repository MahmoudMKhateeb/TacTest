using Abp.Application.Services.Dto;

namespace TACHYON.Countries.CountriesTranslations.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}