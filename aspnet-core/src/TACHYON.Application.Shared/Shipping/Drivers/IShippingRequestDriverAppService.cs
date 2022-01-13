using Abp.Application.Services;
using Abp.Application.Services.Dto;
using DevExtreme.AspNet.Data.ResponseModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using TACHYON.DriverLocationLogs.dtos;
using TACHYON.Routs.RoutPoints.Dtos;
using TACHYON.Shipping.Drivers.Dto;
using TACHYON.Tracking.Dto;

namespace TACHYON.Shipping.Drivers
{
    public interface IShippingRequestDriverAppService : IApplicationService
    {
        Task<PagedResultDto<ShippingRequestTripDriverListDto>> GetAll(ShippingRequestTripDriverFilterInput input);
        Task<ShippingRequestTripDriverDetailsDto> GetDetail(int RequestId, bool IsAccepted);
        Task<RoutDropOffDto> GetDropOffDetail(long PointId);

        Task StartTrip(ShippingRequestTripDriverStartInputDto Input);
        //Task ChangeTripStatus();
        Task GotoNextLocation(long PointId);
        //Task ConfirmReceiverCode(string Code, long? pointId);
        Task SetRating(long PointId, int Rate, string Note);
        Task SetShippingExpRating(int tripId, int rate, string note);
        Task InvokeStatus(InvokeStatusInputDto input);
        Task Accepted(int TripId);
        //Task Rejected(CreateShippingRequestTripDriverRejectDto Input);
        Task Reset(int TripId);
        Task PushNotification(int id,string waybillNumber);
        Task<LoadResult> GetAllDriverLocationLogs(GetAllDriverLocationLogsInput input);



    }

}