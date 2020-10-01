
using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Documents.DocumentTypes.Dtos
{
    public class DocumentTypeDto : EntityDto<long>
    {
        public string DisplayName { get; set; }

        public bool IsRequired { get; set; }

        public bool HasExpirationDate { get; set; }

        public string RequiredFrom { get; set; }

        public bool HasNumber { get; set; }

        public bool HasNotes { get; set; }



    }
}