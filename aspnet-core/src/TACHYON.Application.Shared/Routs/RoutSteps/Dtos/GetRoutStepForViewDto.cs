using TACHYON.Routs.RoutPoints.Dtos;

namespace TACHYON.Routs.RoutSteps.Dtos
{
    public class GetRoutStepForViewDto
    {
        public RoutStepDto RoutStep { get; set; }

        //public string CityDisplayName { get; set; }

        //public string CityDisplayName2 { get; set; }

        public string TrucksTypeDisplayName { get; set; }

        public string TrailerTypeDisplayName { get; set; }

        //public string GoodsDetailName { get; set; }
        public GetRoutPointForViewDto SourceRoutPointDto { get; set; }
        public GetRoutPointForViewDto DestinationRoutPointDto { get; set; }
        public RoutPointGoodsDetailDto SourceRoutePointGoodsDetails { get; set; }
        public RoutPointGoodsDetailDto DestinationRoutPointGoodsDetails { get; set; }
    }
}