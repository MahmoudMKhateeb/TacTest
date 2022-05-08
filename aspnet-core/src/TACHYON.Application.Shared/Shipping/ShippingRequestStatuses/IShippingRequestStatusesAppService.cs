using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.Shipping.ShippingRequestStatuses.Dtos;
using TACHYON.Dto;


namespace TACHYON.Shipping.ShippingRequestStatuses
{
    public interface IShippingRequestStatusesAppService : IApplicationService
    {
        Task<PagedResultDto<GetShippingRequestStatusForViewDto>> GetAll(GetAllShippingRequestStatusesInput input);

        Task<GetShippingRequestStatusForViewDto> GetShippingRequestStatusForView(int id);

        Task<GetShippingRequestStatusForEditOutput> GetShippingRequestStatusForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditShippingRequestStatusDto input);

        Task Delete(EntityDto input);
    }
}