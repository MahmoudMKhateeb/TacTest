
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


        public DateTime ExpirationDate { get; set; }


        public bool HasExpirationDate { get; set; }


        [StringLength(DocumentTypeConsts.MaxRequiredFromLength, MinimumLength = DocumentTypeConsts.MinRequiredFromLength)]
        public string RequiredFrom { get; set; }



    }
}