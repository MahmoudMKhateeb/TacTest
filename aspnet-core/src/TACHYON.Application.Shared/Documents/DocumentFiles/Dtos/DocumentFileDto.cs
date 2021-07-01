
using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Documents.DocumentFiles.Dtos
{
    public class DocumentFileDto : EntityDto<Guid>
    {
        public string Name { get; set; }

        public string Extn { get; set; }

        public Guid? BinaryObjectId { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public bool IsAccepted { get; set; }
        public bool IsRejected { get; set; }
        public string RejectionReason { get; set; }

        public long DocumentTypeId { get; set; }
        public string OtherDocumentTypeName { get; set; }

        public long? TruckId { get; set; }

        public long? TrailerId { get; set; }

        public long? UserId { get; set; }


        public string Number { get; set; }
        public string Notes { get; set; }

        public string HijriExpirationDate { get; set; }
        public virtual DateTime CreationTime { get; set; }


    }
}