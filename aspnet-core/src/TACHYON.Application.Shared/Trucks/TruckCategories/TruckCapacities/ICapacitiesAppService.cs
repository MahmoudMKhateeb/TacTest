using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.Trucks.TruckCategories.TruckCapacities.Dtos;
using TACHYON.Dto;
using System.Collections.Generic;


namespace TACHYON.Trucks.TruckCategories.TruckCapacities
{
    public interface ICapacitiesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetCapacityForViewDto>> GetAll(GetAllCapacitiesInput input);

        Task<GetCapacityForViewDto> GetCapacityForView(int id);

		Task<GetCapacityForEditOutput> GetCapacityForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditCapacityDto input);

		Task Delete(EntityDto input);

		
		Task<List<CapacityTruckSubtypeLookupTableDto>> GetAllTruckSubtypeForTableDropdown();
		
    }
}