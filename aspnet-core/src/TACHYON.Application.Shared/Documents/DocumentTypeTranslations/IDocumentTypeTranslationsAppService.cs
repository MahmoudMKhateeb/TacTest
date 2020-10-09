using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.Documents.DocumentTypeTranslations.Dtos;
using TACHYON.Dto;
using System.Collections.Generic;


namespace TACHYON.Documents.DocumentTypeTranslations
{
    public interface IDocumentTypeTranslationsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetDocumentTypeTranslationForViewDto>> GetAll(GetAllDocumentTypeTranslationsInput input);

        Task<GetDocumentTypeTranslationForViewDto> GetDocumentTypeTranslationForView(int id);

		Task<GetDocumentTypeTranslationForEditOutput> GetDocumentTypeTranslationForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditDocumentTypeTranslationDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetDocumentTypeTranslationsToExcel(GetAllDocumentTypeTranslationsForExcelInput input);

		
		Task<List<DocumentTypeTranslationDocumentTypeLookupTableDto>> GetAllDocumentTypeForTableDropdown();
		
    }
}