using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TACHYON.Routs.RoutPoints.Dtos
{
    public class CreateOrEditRoutPointDto: EntityDto<long?>
    {
        public string DisplayName { get; set; }
        public int? PickingTypeId { get; set; }
        [Required]
        public long FacilityId { get; set; }

        public List<RoutPointGoodsDetailDto> RoutPointGoodsDetailListDto { get; set; }
    }
}
