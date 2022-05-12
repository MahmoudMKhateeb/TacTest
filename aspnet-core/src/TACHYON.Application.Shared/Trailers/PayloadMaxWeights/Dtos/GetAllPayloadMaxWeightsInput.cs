using Abp.Application.Services.Dto;

namespace TACHYON.Trailers.PayloadMaxWeights.Dtos
{
    public class GetAllPayloadMaxWeightsInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string DisplayNameFilter { get; set; }

        public int? MaxMaxWeightFilter { get; set; }
        public int? MinMaxWeightFilter { get; set; }
    }
}