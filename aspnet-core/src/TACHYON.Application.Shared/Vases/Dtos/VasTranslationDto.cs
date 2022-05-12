using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Vases.Dtos
{
    public class VasTranslationDto : EntityDto
    {
        [Required]
        [StringLength(VasConsts.MaxNameLength,
            MinimumLength = VasConsts.MinNameLength)]
        public string DisplayName { get; set; }

        [Required] public string Language { get; set; }

        public int? CoreId { get; set; }
    }
}