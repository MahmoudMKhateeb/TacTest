using Abp.Application.Services.Dto;

namespace TACHYON.DriverLicenseTypes.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}