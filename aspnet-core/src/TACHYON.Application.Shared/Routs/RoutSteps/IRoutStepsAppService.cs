using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.Routs.RoutSteps.Dtos;
using TACHYON.Dto;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Collections.Generic;


namespace TACHYON.Routs.RoutSteps
{
    public interface IRoutStepsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetRoutStepForViewDto>> GetAll(GetAllRoutStepsInput input);

        Task<GetRoutStepForViewDto> GetRoutStepForView(long id);

		Task<GetRoutStepForEditOutput> GetRoutStepForEdit(EntityDto<long> input);

		Task CreateOrEdit(CreateOrEditRoutStepDto input);

		Task Delete(EntityDto<long> input);

		Task<FileDto> GetRoutStepsToExcel(GetAllRoutStepsForExcelInput input);

		
		Task<List<RoutStepCityLookupTableDto>> GetAllCityForTableDropdown();
		
		Task<List<RoutStepRouteLookupTableDto>> GetAllRouteForTableDropdown();
		
    }
}