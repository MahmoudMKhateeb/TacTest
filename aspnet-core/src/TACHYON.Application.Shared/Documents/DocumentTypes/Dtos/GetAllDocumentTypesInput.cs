using Abp.Application.Services.Dto;
using System;
using TACHYON.Documents.DocumentsEntities;

namespace TACHYON.Documents.DocumentTypes.Dtos
{
    public class GetAllDocumentTypesInput
    {
        public string Filter { get; set; }


        //public string DisplayNameFilter { get; set; }

        //public int IsRequiredFilter { get; set; }
        //public int HasExpirationDateFilter { get; set; }

        //public DocumentsEntitiesEnum? RequiredFromFilter { get; set; }
    }
}