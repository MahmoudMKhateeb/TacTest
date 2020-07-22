
using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Routs.RoutTypes.Dtos
{
    public class CreateOrEditRoutTypeDto : EntityDto<int?>
    {

        [Required]
        [StringLength(RoutTypeConsts.MaxDisplayNameLength, MinimumLength = RoutTypeConsts.MinDisplayNameLength)]
        public string DisplayName { get; set; }


        [StringLength(RoutTypeConsts.MaxDescriptionLength, MinimumLength = RoutTypeConsts.MinDescriptionLength)]
        public string Description { get; set; }



    }
}