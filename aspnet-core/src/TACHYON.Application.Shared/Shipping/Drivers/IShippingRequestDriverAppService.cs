using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Common;
using TACHYON.Shipping.Drivers.Dto;

namespace TACHYON.Shipping.Drivers
{
    public interface IShippingRequestDriverAppService: IApplicationService
    {
        Task<PagedResultDto<ShippingRequestTripDriverListDto>> GetAll(ShippingRequestTripDriverFilterInput input);
        Task<bool> StartTrip(long TripId);
        void ChangeTripStatus();
        void GotoNextLocation(long PointId);
        Task<bool> ConfirmReceiverCode(string Code);

        Task<bool> UploadPointDeliveryDocument(ShippingRequestTripDriverDocumentDto Input, string Code);
        void SetRating(long PointId, int Rate);


    }

}
