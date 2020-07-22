
using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Trailers.TrailerStatuses.Dtos
{
    public class CreateOrEditTrailerStatusDto : EntityDto<int?>
    {

        [Required]
        [StringLength(TrailerStatusConsts.MaxDisplayNameLength, MinimumLength = TrailerStatusConsts.MinDisplayNameLength)]
        public string DisplayName { get; set; }



    }
}