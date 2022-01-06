using System.ComponentModel.DataAnnotations;

namespace TACHYON.Rating.dtos
{
    public class CreateFacilityRateByDriverDto : IRatingDto
    {
        [Required]
        public long PointId { get; set; }
        [Range(1, 5)]
        public int Rate { get; set; }
        public string Note { get; set; }
    }
}