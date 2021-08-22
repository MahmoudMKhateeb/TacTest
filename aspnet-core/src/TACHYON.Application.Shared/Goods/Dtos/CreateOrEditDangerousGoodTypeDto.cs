
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Goods.Dtos
{
    public class CreateOrEditDangerousGoodTypeDto : EntityDto<int?>
    {

        [Required]
        [StringLength(DangerousGoodTypeConsts.MaxNameLength, MinimumLength = DangerousGoodTypeConsts.MinNameLength)]
        public string Name { get; set; }


        public int? BayanIntegrationId { get; set; }
    }
}