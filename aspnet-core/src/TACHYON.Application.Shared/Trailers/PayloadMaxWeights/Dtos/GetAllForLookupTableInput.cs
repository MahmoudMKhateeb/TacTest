using Abp.Application.Services.Dto;

namespace TACHYON.Trailers.PayloadMaxWeights.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}