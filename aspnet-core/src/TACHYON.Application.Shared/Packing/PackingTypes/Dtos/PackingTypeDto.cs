using System;
using Abp.Application.Services.Dto;
using System.Collections.Generic;

namespace TACHYON.Packing.PackingTypes.Dtos
{
    public class PackingTypeDto : EntityDto
    {
        // mapped from PackingTypeTranslations
        public string DisplayName { get; set; }

        // mapped from PackingTypeTranslations
        public string Description { get; set; }


    }
}