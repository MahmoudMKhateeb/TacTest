using System;
using Abp.Application.Services.Dto;

namespace TACHYON.EmailTemplates.Dtos
{
    public class EmailTemplateDto : EntityDto
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

    }
}