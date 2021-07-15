using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Editions;
using TACHYON.Documents.DocumentsEntities;
using System.Collections.Generic;
using Abp.AutoMapper;
using TACHYON.Documents.DocumentFiles;
using TACHYON.Documents.DocumentTypeTranslations;
using TACHYON.MultiTenancy;

namespace TACHYON.Documents.DocumentTypes
{
    /// <summary>
    /// Multi-Lingual entity <see cref="DocumentTypeTranslationOld"/>
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
        /// To specify this file type is required from any Edition
        /// </summary>
        public int? EditionId { get; set; }

        [ForeignKey("EditionId")]
        public Edition EditionFk { get; set; }

        public bool HasNumber { get; set; }

        /// <summary>
        /// is the number can duplicate in other entities with the same type? 
        /// </summary>
        public bool IsNumberUnique { get; set; }

        public bool HasNotes { get; set; }

        /// <summary>
        /// to use in filtering or queries or we can use it as a tag
        /// </summary>
        public string SpecialConstant { get; set; }

        /// <summary>
        /// to set min length validation for number <see cref="DocumentFile.Number"/>
        /// </summary>
        public int? NumberMinDigits { get; set; }

        /// <summary>
        /// to set max length validation for number <see cref="DocumentFile.Number"/>
        /// </summary>
        public int? NumberMaxDigits { get; set; }

        /// <summary>
        /// days prior to the expiration date user notified
        /// </summary>
        public int? ExpirationAlertDays { get; set; }
        public bool InActiveAccountExpired { get; set; }

        /// <summary>
        /// tolerance active account Days after expiration date
        /// </summary>
        public int? InActiveToleranceDays { get; set; }

        public bool HasHijriExpirationDate { get; set; }

        public ICollection<DocumentTypeTranslation> Translations { get; set; }

        public string TemplateName { get; set; }
        public string TemplateContentType { get; set; }
        public Guid? TemplateId { get; set; }
        public int? DocumentRelatedWithId { get; set; }

    }
}