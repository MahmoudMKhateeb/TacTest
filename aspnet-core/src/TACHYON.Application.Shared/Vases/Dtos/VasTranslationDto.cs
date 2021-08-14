using System;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Vases.Dtos
{
    public class VasTranslationDto
    {
        [Required]
        [StringLength(VasConsts.MaxNameLength,
            MinimumLength = VasConsts.MinNameLength)]
        public string Name { get; set; }

        [Required]
        public string Language { get; set; }

        public int? CoreId { get; set; }

    }
}
