using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TACHYON.Dto;
using TACHYON.Routs.Dtos;


namespace TACHYON.Routs
{
    public interface IRoutesAppService : IApplicationService
    {
        Task<PagedResultDto<GetRouteForViewDto>> GetAll(GetAllRoutesInput input);

        Task<GetRouteForViewDto> GetRouteForView(int id);

        Task<GetRouteForEditOutput> GetRouteForEdit(EntityDto input);

        Task<int> CreateOrEdit(CreateOrEditRouteDto input);

        Task Delete(EntityDto input);

        Task<FileDto> GetRoutesToExcel(GetAllRoutesForExcelInput input);


        Task<List<RouteRoutTypeLookupTableDto>> GetAllRoutTypeForTableDropdown();

    }
}