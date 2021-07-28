using System;

namespace TACHYON.Documents.DocumentFiles.Dtos
{
    public class GetAllTenantsSubmittedDocumentsDto
    {
        public Guid Id { get; set; }
        public string DocumentTypeName { get; set; }
        public string SubmitterTenatTenancyName { get; set; }
        public DateTime CreationTime { get; set; }
        public string Number { get; set; }
        public string Extn { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool IsAccepted { get; set; }
        public bool IsRejected { get; set; }
    }
}