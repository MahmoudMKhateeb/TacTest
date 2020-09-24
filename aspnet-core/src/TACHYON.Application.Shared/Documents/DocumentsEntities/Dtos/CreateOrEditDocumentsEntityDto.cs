
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Documents.DocumentsEntities.Dtos
{
    public class CreateOrEditDocumentsEntityDto : EntityDto<int?>
    {

		[Required]
		[StringLength(DocumentsEntityConsts.MaxDisplayNameLength, MinimumLength = DocumentsEntityConsts.MinDisplayNameLength)]
		public string DisplayName { get; set; }
		
		

    }
}