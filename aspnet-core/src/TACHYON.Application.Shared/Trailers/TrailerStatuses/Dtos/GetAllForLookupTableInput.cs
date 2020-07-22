using Abp.Application.Services.Dto;

namespace TACHYON.Trailers.TrailerStatuses.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}