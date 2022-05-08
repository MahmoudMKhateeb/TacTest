using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Countries.Dtos
{
    public class CountyDto : EntityDto
    {
        public string DisplayName { get; set; }

        public string Code { get; set; }

        public string TranslatedDisplayName { get; set; }
    }
}