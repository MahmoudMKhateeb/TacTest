using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.Shipping.TripStatuses.Dtos;
using TACHYON.Dto;

namespace TACHYON.Shipping.TripStatuses
{
    public interface ITripStatusesAppService : IApplicationService
    {
        Task<PagedResultDto<GetTripStatusForViewDto>> GetAll(GetAllTripStatusesInput input);

        Task<GetTripStatusForViewDto> GetTripStatusForView(int id);

        Task<GetTripStatusForEditOutput> GetTripStatusForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditTripStatusDto input);

        Task Delete(EntityDto input);

    }
}