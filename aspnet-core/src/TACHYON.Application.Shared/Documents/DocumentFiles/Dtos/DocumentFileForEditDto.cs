using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Documents.DocumentFiles.Dtos
{
    public class DocumentFileForEditDto : EntityDto<Guid?>
    {


        //document details
        public UpdateDocumentFileInput UpdateDocumentFileInput { get; set; }


        //document details section
        public string Name { get; set; }

        public string DocumentTypeDisplayName { get; set; }

        public string Number { get; set; }

        public string Notes { get; set; }

        public DateTime ExpirationDate { get; set; }

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


    }
}