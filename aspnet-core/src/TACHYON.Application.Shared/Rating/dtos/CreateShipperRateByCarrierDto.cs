using System.ComponentModel.DataAnnotations;

namespace TACHYON.Rating.dtos
{
    public class CreateShipperRateByCarrierDto : RatingDto
    {
        [Required] public int TripId { get; set; }
    }
}