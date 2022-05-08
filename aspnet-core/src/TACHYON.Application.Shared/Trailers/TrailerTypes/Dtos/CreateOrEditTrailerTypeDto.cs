using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Trailers.TrailerTypes.Dtos
{
    public class CreateOrEditTrailerTypeDto : EntityDto<int?>
    {
        [Required]
        [StringLength(TrailerTypeConsts.MaxDisplayNameLength, MinimumLength = TrailerTypeConsts.MinDisplayNameLength)]
        public string DisplayName { get; set; }
    }
}