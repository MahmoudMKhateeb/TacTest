using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using TACHYON.Documents.DocumentTypes.Dtos;

namespace TACHYON.Documents.DocumentFiles.Dtos
{
    public class DocumentFileForCreateOrEditDto : EntityDto<Guid?>
    {

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (!DocumentTypeDto.HasNumber)
            {
                return;
            }

            if (DocumentTypeDto.NumberMaxDigits != 0 && Number.ToString().Length > DocumentTypeDto.NumberMaxDigits)
            {
                context.Results.Add(new ValidationResult("Number digits must be less than or equal " + DocumentTypeDto.NumberMaxDigits));
            }

            if (DocumentTypeDto.NumberMinDigits != 0 && Number.ToString().Length < DocumentTypeDto.NumberMinDigits)
            {
                context.Results.Add(new ValidationResult("Number digits must be greater than or equal " + DocumentTypeDto.NumberMinDigits));
            }

        }
        public DocumentTypeDto DocumentTypeDto { get; set; }


        //document details
        public UpdateDocumentFileInput UpdateDocumentFileInput { get; set; }


        //document details section
        public string Name { get; set; }

        public string DocumentTypeDisplayName { get; set; }

        public string Number { get; set; }

        public string Notes { get; set; }

        public DateTime ExpirationDate { get; set; }
        public string HijriExpirationDate { get; set; }


        public string Extn { get; set; }


        //documnet type details section
        public long DocumentTypeId { get; set; }

        public bool HasNumber { get; set; }

        public bool HasNotes { get; set; }
        public bool HasExpirationDate { get; set; }

        public bool IsNumberUnique { get; set; }
        public bool HasHijriExpirationDate { get; set; }

        public int? NumberMinDigits { get; set; }
        public int? NumberMaxDigits { get; set; }

        //Entity ID is the id of the :truck, user,trailer, shipment or any other entity

        public string EntityId { get; set; }
        public string EntityType { get; set; }


    }
}