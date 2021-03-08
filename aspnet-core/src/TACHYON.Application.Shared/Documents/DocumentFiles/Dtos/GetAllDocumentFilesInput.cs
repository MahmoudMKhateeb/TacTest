using Abp.Application.Services.Dto;
using System;
using TACHYON.Documents.DocumentsEntities;

namespace TACHYON.Documents.DocumentFiles.Dtos
{
    public class GetAllDocumentFilesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public DateTime? MaxExpirationDateFilter { get; set; }
        public DateTime? MinExpirationDateFilter { get; set; }

        //public bool? IsAcceptedFilter { get; set; }


        public string DocumentTypeDisplayNameFilter { get; set; }

        public DocumentsEntitiesEnum? DocumentEntityFilter { get; set; }
        public long? TruckIdFilter { get; set; }
        public string EntityIdFilter { get; set; }

        public string TrailerTrailerCodeFilter { get; set; }

        public string UserNameFilter { get; set; }

        //public string RoutStepDisplayNameFilter { get; set; }


    }
}