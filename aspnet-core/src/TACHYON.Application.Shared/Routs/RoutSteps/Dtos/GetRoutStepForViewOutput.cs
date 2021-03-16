using TACHYON.Routs.RoutPoints.Dtos;

namespace TACHYON.Routs.RoutSteps.Dtos
{
    public class GetRoutStepForViewOutput
    {
        public RoutStepDto RoutStep { get; set; }

        public string TrucksTypeDisplayName { get; set; }

        public string TrailerTypeDisplayName { get; set; }

        public GetRoutPointForViewOutput SourceRoutPointDto { get; set; }
        public GetRoutPointForViewOutput DestinationRoutPointDto { get; set; }
    }
}