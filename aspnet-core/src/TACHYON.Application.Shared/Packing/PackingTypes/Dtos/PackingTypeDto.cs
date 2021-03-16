using System;
using Abp.Application.Services.Dto;

namespace TACHYON.Packing.PackingTypes.Dtos
{
    public class PackingTypeDto : EntityDto
    {
        public string DisplayName { get; set; }

        public string Description { get; set; }

    }
}