using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.Trucks.Dtos;
using TACHYON.Dto;
using System.Collections.Generic;
using System.Collections.Generic;


namespace TACHYON.Trucks
{
    public interface ITrucksAppService : IApplicationService 
    {
        Task<PagedResultDto<GetTruckForViewDto>> GetAll(GetAllTrucksInput input);

        Task<GetTruckForViewDto> GetTruckForView(Guid id);

		Task<GetTruckForEditOutput> GetTruckForEdit(EntityDto<Guid> input);

		Task CreateOrEdit(CreateOrEditTruckDto input);

		Task Delete(EntityDto<Guid> input);

		Task<FileDto> GetTrucksToExcel(GetAllTrucksForExcelInput input);

		
		Task<List<TruckTrucksTypeLookupTableDto>> GetAllTrucksTypeForTableDropdown();
		
		Task<List<TruckTruckStatusLookupTableDto>> GetAllTruckStatusForTableDropdown();
		
		Task<PagedResultDto<TruckUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input);
		
    }
}