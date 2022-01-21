namespace TACHYON.Trailers.Dtos
{
    public class GetTrailerForViewDto
    {
        public TrailerDto Trailer { get; set; }

        public string TrailerStatusDisplayName { get; set; }

        public string TrailerTypeDisplayName { get; set; }

        public string PayloadMaxWeightDisplayName { get; set; }

        public string TruckPlateNumber { get; set; }
    }
}