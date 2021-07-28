using Abp.Application.Services.Dto;
using DevExtreme.AspNet.Data;
using System;
using TACHYON.Documents.DocumentsEntities;

namespace TACHYON.Documents.DocumentFiles.Dtos
{
    public class GetAllForListDocumentFilesInput
    {
        public string Filter { get; set; }

    }

    public class GetAllDriversSubmittedDocumentsInput
    {
        public string Filter { get; set; }
        /// <summary>
        /// For view driver documents in all drivers page 
        /// </summary>
        public long? DriverId { get; set; }

    }


    public class GetAllTrucksSubmittedDocumentsInput
    {
        public string Filter { get; set; }
        /// <summary>
        /// For view driver documents in all drivers page 
        /// </summary>
        public long? TruckId { get; set; }

    }
}