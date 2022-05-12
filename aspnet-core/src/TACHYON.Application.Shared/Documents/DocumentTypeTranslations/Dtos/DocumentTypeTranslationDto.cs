using System;
using Abp.Application.Services.Dto;

namespace TACHYON.Documents.DocumentTypeTranslations.Dtos
{
    public class DocumentTypeTranslationDto : EntityDto
    {
        public string Name { get; set; }

        public string Language { get; set; }


        public long CoreId { get; set; }
    }
}