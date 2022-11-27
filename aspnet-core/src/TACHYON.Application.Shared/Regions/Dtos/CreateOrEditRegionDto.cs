using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Regions.Dtos
{
    public class CreateOrEditRegionDto : EntityDto<int?>
    {

        [Required]
        public string Name { get; set; }

        public int BayanIntegrationId { get; set; }

        public int? CountyId { get; set; }

    }
}