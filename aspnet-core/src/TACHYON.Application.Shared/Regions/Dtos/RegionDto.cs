using System;
using Abp.Application.Services.Dto;

namespace TACHYON.Regions.Dtos
{
    public class RegionDto : EntityDto
    {
        public string Name { get; set; }

        public int BayanIntegrationId { get; set; }

        public int? CountyId { get; set; }

    }
}