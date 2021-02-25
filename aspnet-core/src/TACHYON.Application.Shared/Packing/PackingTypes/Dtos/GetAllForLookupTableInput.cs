using Abp.Application.Services.Dto;

namespace TACHYON.Packing.PackingTypes.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}