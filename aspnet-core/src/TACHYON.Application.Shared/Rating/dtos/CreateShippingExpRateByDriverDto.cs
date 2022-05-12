using System.ComponentModel.DataAnnotations;

namespace TACHYON.Rating.dtos
{
    public class CreateShippingExpRateByDriverDto : RatingDto
    {
        [Required] public int TripId { get; set; }
    }
}