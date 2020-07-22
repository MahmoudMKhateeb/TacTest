using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System;
using System.Threading.Tasks;
using TACHYON.Dto;
using TACHYON.Trucks.Dtos;


namespace TACHYON.Trucks
{
    public interface ITruckStatusesAppService : IApplicationService
    {
        Task<PagedResultDto<GetTruckStatusForViewDto>> GetAll(GetAllTruckStatusesInput input);

        Task<GetTruckStatusForViewDto> GetTruckStatusForView(Guid id);

        Task<GetTruckStatusForEditOutput> GetTruckStatusForEdit(EntityDto<Guid> input);

        Task CreateOrEdit(CreateOrEditTruckStatusDto input);

        Task Delete(EntityDto<Guid> input);


    }
}