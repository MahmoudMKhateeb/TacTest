using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.Trucks.TruckCategories.TransportSubtypes.Dtos;
using TACHYON.Dto;
using System.Collections.Generic;


namespace TACHYON.Trucks.TruckCategories.TransportSubtypes
{
    public interface ITransportSubtypesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetTransportSubtypeForViewDto>> GetAll(GetAllTransportSubtypesInput input);

        Task<GetTransportSubtypeForViewDto> GetTransportSubtypeForView(int id);

		Task<GetTransportSubtypeForEditOutput> GetTransportSubtypeForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditTransportSubtypeDto input);

		Task Delete(EntityDto input);

		
		Task<List<TransportSubtypeTransportTypeLookupTableDto>> GetAllTransportTypeForTableDropdown();
		
    }
}