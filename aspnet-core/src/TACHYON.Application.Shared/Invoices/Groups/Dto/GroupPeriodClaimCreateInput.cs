using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TACHYON.Common;
using TACHYON.CustomValidation;

namespace TACHYON.Invoices.Groups.Dto
{
  public  class GroupPeriodClaimCreateInput:Entity<long>, IDocumentUpload
    {
        [Required]
        [UploadBase64File(MaxLength = 1048576 * 100)]
        public string DocumentBase64 { get; set; }

        public Guid? DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentContentType { get; set; }
    }
}
