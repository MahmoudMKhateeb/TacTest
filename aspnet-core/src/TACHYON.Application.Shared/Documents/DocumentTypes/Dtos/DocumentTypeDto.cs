using Abp.Application.Services.Dto;
using System;
using TACHYON.Dto;

namespace TACHYON.Documents.DocumentTypes.Dtos
{
    public class DocumentTypeDto : EntityDto<long>
    {
        /// <summary>
        ///     multiLingual field mapped from DocumentTypeTranslation.Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     from DocumentTypeTranslation.Language
        /// </summary>
        public string Language { get; set; }


        public string DisplayName { get; set; }

        public bool IsRequired { get; set; }

        public bool HasExpirationDate { get; set; }

        public string RequiredFrom { get; set; }
        public int DocumentsEntityId { get; set; }

        public string Edition { get; set; }
        public int? EditionId { get; set; }


        public bool HasNumber { get; set; }

        public bool HasNotes { get; set; }

        public bool IsNumberUnique { get; set; }
        public string SpecialConstant { get; set; }
        public int? NumberMinDigits { get; set; }

        public int? NumberMaxDigits { get; set; }
        public int? ExpirationAlertDays { get; set; }
        public bool InActiveAccountExpired { get; set; }
        public int? InActiveToleranceDays { get; set; }

        public bool HasHijriExpirationDate { get; set; }
        public bool IsRequiredNumber { get; set; }
        public bool IsRequiredExpirationDate { get; set; }
        public bool IsRequiredDocumentTemplate { get; set; }

        public string TemplateName { get; set; }
        public string TemplateContentType { get; set; }
        public Guid? TemplateId { get; set; }
        public int? DocumentRelatedWithId { get; set; }
        public string DocumentRelatedWithName { get; set; }
    }
}