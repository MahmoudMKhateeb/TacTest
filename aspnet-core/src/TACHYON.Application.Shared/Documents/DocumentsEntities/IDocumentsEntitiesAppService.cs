using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.Documents.DocumentsEntities.Dtos;
using TACHYON.Dto;


namespace TACHYON.Documents.DocumentsEntities
{
    public interface IDocumentsEntitiesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetDocumentsEntityForViewDto>> GetAll(GetAllDocumentsEntitiesInput input);

        Task<GetDocumentsEntityForViewDto> GetDocumentsEntityForView(int id);

		Task<GetDocumentsEntityForEditOutput> GetDocumentsEntityForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditDocumentsEntityDto input);

		Task Delete(EntityDto input);

		
    }
}