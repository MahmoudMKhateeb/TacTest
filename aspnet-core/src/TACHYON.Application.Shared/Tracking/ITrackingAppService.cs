using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using TACHYON.Dto;
using TACHYON.Shipping.Drivers.Dto;
using TACHYON.Tracking.Dto;
using TACHYON.Tracking.Dto.WorkFlow;

namespace TACHYON.Tracking
{
    public interface ITrackingAppService : IApplicationService
    {
        Task<PagedResultDto<TrackingListDto>> GetAll(TrackingSearchInputDto Input);
        Task<ListResultDto<ShippingRequestTripDriverRoutePointDto>> GetForView(long id);
        Task Accept(int id);
        Task Start(int id);
        Task InvokeStatus(InvokeStatusInputDto input);
        Task NextLocation(long id);
        Task<FileDto> POD(long id);
    }
}