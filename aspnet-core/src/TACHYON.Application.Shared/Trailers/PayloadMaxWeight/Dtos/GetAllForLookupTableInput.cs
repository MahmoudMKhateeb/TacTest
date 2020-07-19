using Abp.Application.Services.Dto;

namespace TACHYON.Trailers.PayloadMaxWeight.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}