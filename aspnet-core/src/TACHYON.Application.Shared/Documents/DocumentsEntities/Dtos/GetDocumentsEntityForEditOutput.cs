using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Documents.DocumentsEntities.Dtos
{
    public class GetDocumentsEntityForEditOutput
    {
        public CreateOrEditDocumentsEntityDto DocumentsEntity { get; set; }
    }
}