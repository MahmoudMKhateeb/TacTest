using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TACHYON.Dto;
using TACHYON.Trucks.TruckCategories.TransportSubtypes.Dtos;
using TACHYON.Trucks.TrucksTypes.Dtos;


namespace TACHYON.Trucks.TrucksTypes
{
    public interface ITrucksTypesAppService : IApplicationService
    {
        Task<PagedResultDto<GetTrucksTypeForViewDto>> GetAll(GetAllTrucksTypesInput input);

        Task<GetTrucksTypeForViewDto> GetTrucksTypeForView(long id);

        Task<GetTrucksTypeForEditOutput> GetTrucksTypeForEdit(EntityDto<long> input);

        Task CreateOrEdit(CreateOrEditTrucksTypeDto input);

        Task Delete(EntityDto<long> input);
        Task<List<TransportSubtypeTransportTypeLookupTableDto>> GetAllTransportSubTypeForTableDropdown();


    }
}