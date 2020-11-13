using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TACHYON.Documents.DocumentFiles.Dtos;
using TACHYON.Documents.DocumentsEntities.Dtos;
using TACHYON.Dto;


namespace TACHYON.Documents.DocumentFiles
{
    public interface IDocumentFilesAppService : IApplicationService
    {
        Task<PagedResultDto<GetDocumentFileForViewDto>> GetAll(GetAllDocumentFilesInput input);

        Task<GetDocumentFileForViewDto> GetDocumentFileForView(Guid id);

        Task<GetDocumentFileForEditOutput> GetDocumentFileForEdit(EntityDto<Guid> input);

        Task CreateOrEdit(CreateOrEditDocumentFileDto input);

        Task Delete(EntityDto<Guid> input);

        //Task<FileDto> GetDocumentFilesToExcel(GetAllDocumentFilesForExcelInput input);


        Task<List<DocumentFileDocumentTypeLookupTableDto>> GetAllDocumentTypeForTableDropdown();

        Task<List<DocumentFileTruckLookupTableDto>> GetAllTruckForTableDropdown();

        Task<List<DocumentFileTrailerLookupTableDto>> GetAllTrailerForTableDropdown();

        Task<List<DocumentFileUserLookupTableDto>> GetAllUserForTableDropdown();

        Task<List<DocumentFileRoutStepLookupTableDto>> GetAllRoutStepForTableDropdown();

        Task<FileDto> GetDocumentFileDto(Guid documentFileId);

        //Task<List<GetDocumentEntitiesLookupDto>> GetDocumentEntitiesForDocumentFile();
        Task<List<CreateOrEditDocumentFileDto>> GetDriverRequiredDocumentFiles(string userId);
        Task<List<CreateOrEditDocumentFileDto>> GetTruckRequiredDocumentFiles(string truckId);

        //bool GetIsCurrentTenantHost();

    }
}