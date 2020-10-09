using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Documents.DocumentTypeTranslations.Dtos
{
    public class GetDocumentTypeTranslationForEditOutput
    {
		public CreateOrEditDocumentTypeTranslationDto DocumentTypeTranslation { get; set; }

		public string DocumentTypeDisplayName { get; set;}


    }
}