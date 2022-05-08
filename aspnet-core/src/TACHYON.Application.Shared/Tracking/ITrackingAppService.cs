using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;
using TACHYON.Documents.DocumentFiles.Dtos;
using TACHYON.Common;
using TACHYON.Dto;
using TACHYON.Shipping.Drivers.Dto;
using TACHYON.Tracking.Dto;
using TACHYON.Tracking.Dto.WorkFlow;

namespace TACHYON.Tracking
{
    public interface ITrackingAppService : IApplicationService
    {
        Task<PagedResultDto<TrackingListDto>> GetAll(TrackingSearchInputDto Input);
        Task<TrackingShippingRequestTripDto> GetForView(long id);
        Task Accept(int id);
        Task Start(int id);
        Task InvokeStatus(InvokeStatusInputDto input);
        Task NextLocation(long id);
        Task<List<GetAllUploadedFileDto>> POD(long id);
        Task<IHasDocument> GetDeliveryGoodPicture(long id);
    }
}