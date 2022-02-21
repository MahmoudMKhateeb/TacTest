using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.EmailTemplates.Dtos
{
    public class EmailTemplateTranslationDto : EntityDto
    {
        public virtual string TranslatedContent { get; set; }

        [Required]
        public virtual string Language { get; set; }
    }

    public class CreateOrEditEmailTemplateTranslationDto  : EntityDto<int?>
    {
        public string TranslatedContent { get; set; }

        [Required]
        public string Language { get; set; }

        public int CoreId { get; set; }



    }

    public class GetEmailTemplateTranslationForEditOutput
    {
        public CreateOrEditEmailTemplateTranslationDto EmailTemplate { get; set; }
    }
}
