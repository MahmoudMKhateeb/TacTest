using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Editions;
using TACHYON.Documents.DocumentsEntities;
using System.Collections.Generic;
using Abp.AutoMapper;

namespace TACHYON.Documents.DocumentTypes
{
    /// <summary>
    /// Multi-Lingual entity <see cref="DocumentTypeTranslation"/>
    /// </summary>
    [Table("DocumentTypes")]
    public class DocumentType : FullAuditedEntity<long>, IMultiLingualEntity<DocumentTypeTranslation>
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

        public ICollection<DocumentTypeTranslation> Translations { get; set; }
    }
}