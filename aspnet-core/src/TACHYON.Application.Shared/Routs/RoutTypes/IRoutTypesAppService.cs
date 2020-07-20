using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.Routs.RoutTypes.Dtos;
using TACHYON.Dto;


namespace TACHYON.Routs.RoutTypes
{
    public interface IRoutTypesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetRoutTypeForViewDto>> GetAll(GetAllRoutTypesInput input);

        Task<GetRoutTypeForViewDto> GetRoutTypeForView(int id);

		Task<GetRoutTypeForEditOutput> GetRoutTypeForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditRoutTypeDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetRoutTypesToExcel(GetAllRoutTypesForExcelInput input);

		
    }
}