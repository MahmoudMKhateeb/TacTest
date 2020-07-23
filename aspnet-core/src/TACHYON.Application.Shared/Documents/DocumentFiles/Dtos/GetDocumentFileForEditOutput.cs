using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Documents.DocumentFiles.Dtos
{
    public class GetDocumentFileForEditOutput
    {
        public CreateOrEditDocumentFileDto DocumentFile { get; set; }

        public string DocumentTypeDisplayName { get; set; }

        public string TruckPlateNumber { get; set; }

        public string TrailerTrailerCode { get; set; }

        public string UserName { get; set; }

        public string RoutStepDisplayName { get; set; }


    }
}