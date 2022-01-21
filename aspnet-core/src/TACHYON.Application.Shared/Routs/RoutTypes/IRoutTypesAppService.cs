using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System;
using System.Threading.Tasks;
using TACHYON.Dto;
using TACHYON.Routs.RoutTypes.Dtos;


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