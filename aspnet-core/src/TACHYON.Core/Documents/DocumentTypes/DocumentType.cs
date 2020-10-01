using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Editions;
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

        public virtual bool HasExpirationDate { get; set; }

        public int DocumentsEntityId { get; set; }

        [ForeignKey("DocumentsEntityId")]
        public DocumentsEntity DocumentsEntityFk { get; set; }

        /// <summary>
        /// To specify this file is required from any Edition
        /// </summary>
        public int? EditionId { get; set; }

        [ForeignKey("EditionId")]
        public Edition EditionFk { get; set; }

        public bool HasNumber { get; set; }

        public bool HasNotes { get; set; }

    }
}