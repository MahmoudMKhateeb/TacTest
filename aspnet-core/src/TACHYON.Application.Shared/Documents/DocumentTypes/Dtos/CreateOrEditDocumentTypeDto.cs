
using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

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

        public bool HasNotes { get; set; }



    }
}