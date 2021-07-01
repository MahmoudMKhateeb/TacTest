using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace TACHYON.Trucks.PlateTypes.Dtos
{
    public class PlateTypeDto : EntityDto
    {
        //return displayName bt current user language
        public int? BayanPlatetypeId { get; set; }
        public string DisplayName { get; set; }


    }
}