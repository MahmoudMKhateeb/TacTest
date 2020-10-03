using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Documents.DocumentTypeTranslations.Dtos
{
    public class GetAllDocumentTypeTranslationsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string NameFilter { get; set; }

		public string LanguageFilter { get; set; }


		 public string DocumentTypeDisplayNameFilter { get; set; }

		 

    }
}