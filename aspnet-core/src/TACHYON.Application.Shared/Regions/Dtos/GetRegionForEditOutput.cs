using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Regions.Dtos
{
    public class GetRegionForEditOutput
    {
        public CreateOrEditRegionDto Region { get; set; }

        public string CountyDisplayName { get; set; }

    }
}