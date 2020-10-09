using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Documents.DocumentFiles.Dtos
{
    public class GetAllDocumentFilesForExcelInput
    {
        public string Filter { get; set; }

        public string NameFilter { get; set; }

        public string ExtnFilter { get; set; }

        public Guid? BinaryObjectIdFilter { get; set; }

        public DateTime? MaxExpirationDateFilter { get; set; }
        public DateTime? MinExpirationDateFilter { get; set; }

        public bool? IsAcceptedFilter { get; set; }


        public string DocumentTypeDisplayNameFilter { get; set; }

        public string TruckPlateNumberFilter { get; set; }

        public string TrailerTrailerCodeFilter { get; set; }

        public string UserNameFilter { get; set; }

        public string RoutStepDisplayNameFilter { get; set; }


    }
}