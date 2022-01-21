using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Documents.DocumentTypeTranslations.Dtos
{
    public class CreateOrEditDocumentTypeTranslationDto : EntityDto<int?>
    {
        [Required]
        [StringLength(DocumentTypeTranslationConsts.MaxNameLength,
            MinimumLength = DocumentTypeTranslationConsts.MinNameLength)]
        public string Name { get; set; }


        [Required] public string Language { get; set; }


        public long CoreId { get; set; }
    }
}