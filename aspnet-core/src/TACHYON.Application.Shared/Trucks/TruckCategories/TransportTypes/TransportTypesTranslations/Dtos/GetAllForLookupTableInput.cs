using Abp.Application.Services.Dto;

namespace TACHYON.Trucks.TruckCategories.TransportTypes.TransportTypesTranslations.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}