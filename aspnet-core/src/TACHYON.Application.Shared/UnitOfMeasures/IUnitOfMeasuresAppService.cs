using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.UnitOfMeasures.Dtos;
using TACHYON.Dto;


namespace TACHYON.UnitOfMeasures
{
    public interface IUnitOfMeasuresAppService : IApplicationService 
    {
        Task<PagedResultDto<GetUnitOfMeasureForViewDto>> GetAll(GetAllUnitOfMeasuresInput input);

        Task<GetUnitOfMeasureForViewDto> GetUnitOfMeasureForView(int id);

		Task<GetUnitOfMeasureForEditOutput> GetUnitOfMeasureForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditUnitOfMeasureDto input);

		Task Delete(EntityDto input);

		
    }
}