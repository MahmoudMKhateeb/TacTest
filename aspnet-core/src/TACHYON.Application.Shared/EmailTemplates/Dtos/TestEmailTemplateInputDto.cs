using System.ComponentModel.DataAnnotations;

namespace TACHYON.EmailTemplates.Dtos
{
    public class TestEmailTemplateInputDto
    {
        public CreateOrEditEmailTemplateDto TestTemplate { get; set; }
        
        [Required]
        [DataType(DataType.EmailAddress)]
        public string TestEmail { get; set; }
    }
}