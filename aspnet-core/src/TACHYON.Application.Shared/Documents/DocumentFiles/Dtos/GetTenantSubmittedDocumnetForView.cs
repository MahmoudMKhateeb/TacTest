using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Documents.DocumentFiles.Dtos
{
   public class GetTenantSubmittedDocumnetForView
    {
        public virtual Guid Id { get; set; }

        public virtual string Name { get; set; }
        public virtual string Extn { get; set; }

        public virtual bool IsAccepted { get; set; }
        public virtual bool IsRejected { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}
