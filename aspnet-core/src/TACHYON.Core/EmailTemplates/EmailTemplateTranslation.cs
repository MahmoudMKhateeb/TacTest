using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TACHYON.EmailTemplates
{
    public class EmailTemplateTranslation : FullAuditedEntity, IEntityTranslation<EmailTemplate>
    {
        public virtual string TranslatedContent { get; set; }

        [Required]
        public virtual string Language { get; set; }

        public virtual int CoreId { get; set; }

        [ForeignKey("CoreId")]
        public EmailTemplate Core { get; set; }
    }
}