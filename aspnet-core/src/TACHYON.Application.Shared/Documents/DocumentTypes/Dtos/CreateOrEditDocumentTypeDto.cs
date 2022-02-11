using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;
using TACHYON.Dto;

namespace TACHYON.Documents.DocumentTypes.Dtos
{
    public class CreateOrEditDocumentTypeDto : EntityDto<long?>
    {
        [Required]
        [StringLength(DocumentTypeConsts.MaxDisplayNameLength, MinimumLength = DocumentTypeConsts.MinDisplayNameLength)]
        public string DisplayName { get; set; }


        public bool IsRequired { get; set; }

        public bool HasExpirationDate { get; set; }

        public int DocumentsEntityId { get; set; }

        public int? EditionId { get; set; }
        public bool HasNumber { get; set; }
        public bool IsRequiredNumber { get; set; }
        public bool IsRequiredExpirationDate { get; set; }
        public bool IsRequiredDocumentTemplate { get; set; }
        public bool HasNotes { get; set; }
        public bool IsNumberUnique { get; set; }
        public string SpecialConstant { get; set; }
        public int? NumberMinDigits { get; set; }
        public int? NumberMaxDigits { get; set; }
        public int? ExpirationAlertDays { get; set; }
        public bool InActiveAccountExpired { get; set; }
        public int? InActiveToleranceDays { get; set; }
        public bool HasHijriExpirationDate { get; set; }


        public string TemplateBase64 { get; set; }

        public string TemplateName { get; set; }
        public string TemplateContentType { get; set; }
        public string TemplateExt { get; set; }
        public Guid? TemplateId { get; set; }
        /* public int? TenantId { get; set; }*/
        public int? DocumentRelatedWithId { get; set; }
        public SelectItemDto DocumentRelatedWith { get; set; }

        [MaxLength(400)]
        public string FileToken { get; set; }

    }
}