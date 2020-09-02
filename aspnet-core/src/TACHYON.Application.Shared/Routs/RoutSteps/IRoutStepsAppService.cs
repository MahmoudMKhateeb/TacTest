using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Threading.Tasks;
using TACHYON.Dto;
using TACHYON.Routs.RoutSteps.Dtos;


namespace TACHYON.Routs.RoutSteps
{
    public interface IRoutStepsAppService : IApplicationService
    {
        Task<PagedResultDto<GetRoutStepForViewDto>> GetAll(GetAllRoutStepsInput input);

        Task<GetRoutStepForViewDto> GetRoutStepForView(long id);

        Task<GetRoutStepForEditOutput> GetRoutStepForEdit(EntityDto<long> input);

        Task CreateOrEdit(CreateOrEditRoutStepDto input);

        Task Delete(EntityDto<long> input);

        //Task<FileDto> GetRoutStepsToExcel(GetAllRoutStepsForExcelInput input);


        Task<List<RoutStepCityLookupTableDto>> GetAllCityForTableDropdown();

        Task<List<RoutStepRouteLookupTableDto>> GetAllRouteForTableDropdown();

    }
}