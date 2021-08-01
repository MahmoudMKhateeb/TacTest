using System;

namespace TACHYON.Documents.DocumentFiles.Dtos
{
    public class GetAllTrucksSubmittedDocumentsDto
    {
        public Guid Id { get; set; }
        public string DocumentTypeName { get; set; }
        public long TruckId { get; set; }
        public string TruckPlateNumber { get; set; }
        public DateTime CreationTime { get; set; }
        public string Number { get; set; }
        public string Extn { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool IsAccepted { get; set; }
        public bool IsRejected { get; set; }
    }
}