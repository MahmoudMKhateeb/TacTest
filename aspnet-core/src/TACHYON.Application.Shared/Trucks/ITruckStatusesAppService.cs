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

        Task<GetTruckStatusForViewDto> GetTruckStatusForView(long id);

        Task<GetTruckStatusForEditOutput> GetTruckStatusForEdit(EntityDto<long> input);

        Task CreateOrEdit(CreateOrEditTruckStatusDto input);

        Task Delete(EntityDto<long> input);


    }
}