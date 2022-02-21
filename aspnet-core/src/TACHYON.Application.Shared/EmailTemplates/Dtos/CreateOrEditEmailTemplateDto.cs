using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.EmailTemplates.Dtos
{
    public class CreateOrEditEmailTemplateDto : EntityDto<int?>
    {

        [Required]
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Content { get; set; }

        public virtual string Html { get; set; }

        public virtual EmailTemplateTypesEnum EmailTemplateType { get; set; }
        public string Tags { get; set; }
        public string Description { get; set; }


    }
}