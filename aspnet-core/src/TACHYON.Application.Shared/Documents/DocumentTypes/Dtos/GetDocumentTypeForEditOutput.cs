using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Documents.DocumentTypes.Dtos
{
    public class GetDocumentTypeForEditOutput
    {
        public CreateOrEditDocumentTypeDto DocumentType { get; set; }


    }
}