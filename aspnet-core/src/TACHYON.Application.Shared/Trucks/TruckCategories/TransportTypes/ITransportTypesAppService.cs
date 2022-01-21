using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.Trucks.TruckCategories.TransportTypes.Dtos;
using TACHYON.Dto;


namespace TACHYON.Trucks.TruckCategories.TransportTypes
{
    public interface ITransportTypesAppService : IApplicationService
    {
        Task<PagedResultDto<GetTransportTypeForViewDto>> GetAll(GetAllTransportTypesInput input);

        Task<GetTransportTypeForViewDto> GetTransportTypeForView(int id);

        Task<GetTransportTypeForEditOutput> GetTransportTypeForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditTransportTypeDto input);

        Task Delete(EntityDto input);
    }
}