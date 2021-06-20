using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using TACHYON.Tracking.Dto;

namespace TACHYON.Tracking
{
    public interface ITrackingAppService:IApplicationService
    {
        Task<PagedResultDto<TrackingListDto>> GetAll(TrackingSearchInputDto Input);
    }
}
