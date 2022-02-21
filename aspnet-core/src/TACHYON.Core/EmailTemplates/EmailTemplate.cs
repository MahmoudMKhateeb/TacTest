using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using System.Collections.Generic;

namespace TACHYON.EmailTemplates
{
    [Table("EmailTemplates")]
    public class EmailTemplate : FullAuditedEntity ,IMultiLingualEntity<EmailTemplateTranslation>
    {

        [Required]
        public virtual string Name { get; set; }

        public virtual string DisplayName { get; set; }

        public virtual string Content { get; set; }

        public ICollection<EmailTemplateTranslation> Translations { get; set; }

        public virtual EmailTemplateTypesEnum EmailTemplateType { get; set; }
        public string Tags { get; set; }
        public string Description { get; set; }
    }
}