using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Shipping.Trips.Dto;

namespace TACHYON.Shipping.Trips
{
    public interface IShippingRequestsTripAppService : IApplicationService
    {

        Task<PagedResultDto<ShippingRequestsTripListDto>> GetAll(ShippingRequestTripFilterInput Input);


        Task<ShippingRequestsTripForViewDto> GetShippingRequestTripForView(int id);

        Task CreateOrEdit(CreateOrEditShippingRequestTripDto input);
        Task<CreateOrEditShippingRequestTripDto> GetShippingRequestTripForEdit(EntityDto input);

        Task Delete(EntityDto input);
        Task CancelByAccident(long Id, bool isForce);
    }
}