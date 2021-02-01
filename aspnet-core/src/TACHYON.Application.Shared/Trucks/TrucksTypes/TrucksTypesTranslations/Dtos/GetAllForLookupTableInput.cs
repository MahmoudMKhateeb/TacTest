using Abp.Application.Services.Dto;

namespace TACHYON.Trucks.TrucksTypes.TrucksTypesTranslations.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}