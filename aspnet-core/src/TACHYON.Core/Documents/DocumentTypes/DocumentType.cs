using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Documents.DocumentsEntities;

namespace TACHYON.Documents.DocumentTypes
{
    [Table("DocumentTypes")]
    public class DocumentType : FullAuditedEntity<long>
    {

        [Required]
        [StringLength(DocumentTypeConsts.MaxDisplayNameLength, MinimumLength = DocumentTypeConsts.MinDisplayNameLength)]
        public virtual string DisplayName { get; set; }

        public virtual bool IsRequired { get; set; }

        public virtual DateTime ExpirationDate { get; set; }

        public virtual bool HasExpirationDate { get; set; }

        [StringLength(DocumentTypeConsts.MaxRequiredFromLength, MinimumLength = DocumentTypeConsts.MinRequiredFromLength)]
        public virtual string RequiredFrom { get; set; }

        public int DocumentsEntityId { get; set; }

        [ForeignKey("DocumentsEntityId")]
        public DocumentsEntity DocumentsEntityFk { get; set; }


    }
}