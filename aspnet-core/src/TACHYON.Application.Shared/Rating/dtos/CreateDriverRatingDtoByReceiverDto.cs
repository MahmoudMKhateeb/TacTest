using System.ComponentModel.DataAnnotations;

namespace TACHYON.Rating.dtos
{
    public class CreateDriverRatingDtoByReceiverDto: IRatingDto
    {
        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string Code { get; set; }
        [Range(1, 5)]
        public int Rate { get; set; }
        public string Note { get; set; }
    }
}
