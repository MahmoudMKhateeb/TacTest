using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Routs.RoutTypes.Dtos
{
    public class RoutTypeDto : EntityDto
    {
        public string DisplayName { get; set; }

        public string Description { get; set; }
    }
}