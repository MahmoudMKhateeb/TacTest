using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Trucks.PlateTypes.Dtos
{
    public class PlateTypeDto : EntityDto
    {

        public virtual string Name { get; set; }

        public string BayanIntegrationId { get; set; }

        //return displayName bt current user language
        public string DisplayName { get; set; }


    }
}