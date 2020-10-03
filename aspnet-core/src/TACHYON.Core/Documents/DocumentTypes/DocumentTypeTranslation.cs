using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.AutoMapper;
using Abp.Domain.Entities;

namespace TACHYON.Documents.DocumentTypes
{
    [Table("DocumentTypeTranslations")]
    public class DocumentTypeTranslation : Entity, IEntityTranslation<DocumentType, long>
    {
        [Required]
        [StringLength(DocumentTypeConsts.MaxDisplayNameLength, MinimumLength = DocumentTypeConsts.MinDisplayNameLength)]
        public virtual string Name { get; set; }

        public string Language { get; set; }
        public long CoreId { get; set; }

        [ForeignKey("CoreId")]
        public DocumentType Core { get; set; }
    }
}