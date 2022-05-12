using TACHYON.Documents.DocumentTypes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace TACHYON.Documents.DocumentTypeTranslations
{
    [Table("DocumentTypeTranslations")]
    public class DocumentTypeTranslation : Entity, IEntityTranslation<DocumentType, long>
    {
        [Required]
        [StringLength(DocumentTypeTranslationConsts.MaxNameLength,
            MinimumLength = DocumentTypeTranslationConsts.MinNameLength)]
        public virtual string Name { get; set; }

        [Required] public virtual string Language { get; set; }


        public virtual long CoreId { get; set; }

        [ForeignKey("CoreId")] public DocumentType Core { get; set; }
    }
}