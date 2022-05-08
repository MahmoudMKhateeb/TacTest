using System.ComponentModel.DataAnnotations;

namespace TACHYON.Rating.dtos
{
    public class CreateDriverRatingDtoByReceiverDto : RatingDto
    {
        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string Code { get; set; }
    }
}