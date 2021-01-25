using Abp.Application.Services.Dto;

namespace TACHYON.Trucks.TruckStatusesTranslations.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}