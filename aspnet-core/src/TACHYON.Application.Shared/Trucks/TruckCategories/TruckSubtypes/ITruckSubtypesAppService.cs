using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.Trucks.TruckCategories.TruckSubtypes.Dtos;
using TACHYON.Dto;
using System.Collections.Generic;


namespace TACHYON.Trucks.TruckCategories.TruckSubtypes
{
    public interface ITruckSubtypesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetTruckSubtypeForViewDto>> GetAll(GetAllTruckSubtypesInput input);

        Task<GetTruckSubtypeForViewDto> GetTruckSubtypeForView(int id);

		Task<GetTruckSubtypeForEditOutput> GetTruckSubtypeForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditTruckSubtypeDto input);

		Task Delete(EntityDto input);

		
		Task<List<TruckSubtypeTrucksTypeLookupTableDto>> GetAllTrucksTypeForTableDropdown();
		
    }
}