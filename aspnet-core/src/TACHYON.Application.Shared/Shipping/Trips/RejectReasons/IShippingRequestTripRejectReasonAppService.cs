using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using TACHYON.Common.Dto;
using TACHYON.Shipping.Trips.RejectReasons.Dtos;

namespace TACHYON.Shipping.Trips.RejectReasons
{
    public interface IShippingRequestTripRejectReasonAppService : IApplicationService
    {
        ListResultDto<ShippingRequestTripRejectReasonListDto> GetAllRejectReason(FilterInput Input);
        Task<CreateOrEditShippingRequestTripRejectReasonDto> GetForEdit(EntityDto input);
        Task CreateOrEdit(CreateOrEditShippingRequestTripRejectReasonDto input);
        Task Delete(EntityDto input);
    }
}
