using Abp.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using TACHYON.Common;
using TACHYON.CustomValidation;

namespace TACHYON.Invoices.SubmitInvoices.Dto
{
    public  class SubmitInvoiceClaimCreateInput:Entity<long>, IDocumentUpload
    {
        [Required]
        [UploadBase64File(MaxLength = 1048576 * 100)]
        public string DocumentBase64 { get; set; }

        public Guid? DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentContentType { get; set; }
    }
}
