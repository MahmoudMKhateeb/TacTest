using System.ComponentModel.DataAnnotations;

namespace TACHYON.Rating.dtos
{
    public class CreateFacilityRateByDriverDto : RatingDto
    {
        [Required]
        public long PointId { get; set; }
    }
}