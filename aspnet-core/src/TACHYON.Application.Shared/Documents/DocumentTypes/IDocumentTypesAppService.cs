﻿using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System;
using System.Threading.Tasks;
using TACHYON.Documents.DocumentTypes.Dtos;
using TACHYON.Dto;


namespace TACHYON.Documents.DocumentTypes
{
    public interface IDocumentTypesAppService : IApplicationService
    {
        Task<PagedResultDto<GetDocumentTypeForViewDto>> GetAll(GetAllDocumentTypesInput input);

        Task<GetDocumentTypeForViewDto> GetDocumentTypeForView(long id);

        Task<GetDocumentTypeForEditOutput> GetDocumentTypeForEdit(EntityDto<long> input);

        Task CreateOrEdit(CreateOrEditDocumentTypeDto input);

        Task Delete(EntityDto<long> input);

        Task<FileDto> GetDocumentTypesToExcel(GetAllDocumentTypesForExcelInput input);


    }
}