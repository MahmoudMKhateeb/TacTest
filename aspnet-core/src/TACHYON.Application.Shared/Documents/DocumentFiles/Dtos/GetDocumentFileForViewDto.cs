using System;

namespace TACHYON.Documents.DocumentFiles.Dtos
{
    public class GetDocumentFileForViewDto
    {
        public DocumentFileDto DocumentFile { get; set; }
        public string SubmitterTenatTenancyName { get; set; }
        public string DocumentEntityDisplayName { get; set; }
        public string Note { get; set; }
        public string Number { get; set; }
        public bool HasDate { get; set; }
        public bool HasNumber { get; set; }
        public string DocumentTypeDisplayName { get; set; }

        public string TruckId { get; set; }
        public string PlateNumber { get; set; }

        public string TrailerTrailerCode { get; set; }

        public string UserName { get; set; }

        public string RoutStepDisplayName { get; set; }
        public UserInGetDocumentFileForViewDto User { get; set; }
    }
}