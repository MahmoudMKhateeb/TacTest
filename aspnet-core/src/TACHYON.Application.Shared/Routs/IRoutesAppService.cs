using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.Routs.Dtos;
using TACHYON.Dto;
using System.Collections.Generic;


namespace TACHYON.Routs
{
    public interface IRoutesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetRouteForViewDto>> GetAll(GetAllRoutesInput input);

        Task<GetRouteForViewDto> GetRouteForView(int id);

		Task<GetRouteForEditOutput> GetRouteForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditRouteDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetRoutesToExcel(GetAllRoutesForExcelInput input);

		
		Task<List<RouteRoutTypeLookupTableDto>> GetAllRoutTypeForTableDropdown();
		
    }
}